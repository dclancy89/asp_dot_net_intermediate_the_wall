using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using The_Wall.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace The_Wall.Controllers
{
    public class HomeController : Controller
    {

        private readonly DbConnector _dbConnector;
 
        public HomeController(DbConnector connect)
        {
            _dbConnector = connect;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            ViewBag.Register = new RegisterViewModel();
            ViewBag.Login = new LoginUser();
            return View();
        }

        [HttpPost]
        [Route("register")]
        public IActionResult Register(RegisterViewModel model)
        {
            if(ModelState.IsValid)
            {
                RegisterUser NewUser = new RegisterUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Password = model.Password
                };

                RegisterUser(NewUser);
                return RedirectToAction("Dashboard");
            }
            else
            {
                return RedirectToAction("Index");
            }
            
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login(LoginUser user)
        {
            List<Dictionary<string,object>> users = _dbConnector.Query($"SELECT id, Password FROM Users WHERE Email='{user.LogEmail}'");
            PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();

            if((users.Count == 0 || user.LogPassword == null) || hasher.VerifyHashedPassword(user, (string)users[0]["Password"], user.LogPassword) == 0)
            {
                ModelState.AddModelError("LogEmail", "Invalid Email/Password");
            }
            if(ModelState.IsValid)
            {
                HttpContext.Session.SetInt32("id", (int)users[0]["id"]);
                return RedirectToAction("Dashboard");
            }

            return View("Index");
        }

        [HttpGet]
        [Route("dashboard")]
        public IActionResult Dashboard()
        {

            if (HttpContext.Session.GetInt32("id") == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                int id = (int)HttpContext.Session.GetInt32("id");

                ViewBag.Message = new MessageViewModel();
                ViewBag.Comment = new CommentViewModel();
                ViewBag.ID = id;
                ViewBag.FirstName = _dbConnector.Query($"SELECT * FROM Users WHERE id='{id}';")[0]["FirstName"];
                
                string MessageQuery = $@"SELECT Users.FirstName AS FirstName, Users.LastName AS LastName, Messages.id AS id, Messages.UserID AS UserID, Messages.MessageText AS MessageText, DATE_FORMAT(Messages.CreatedAt, '%b %d, %Y') as CreatedAt 
                                FROM Messages 
                                JOIN Users ON Users.id = Messages.UserID
                                ORDER BY Messages.CreatedAt DESC";
                List<Dictionary<string,object>> messages = _dbConnector.Query(MessageQuery);

                if(messages.Count == 0)
                {
                    List<Dictionary<string,string>> DefaultMessage = new List<Dictionary<string,string>>();
                    Dictionary<string,string> m = new Dictionary<string,string>();
                    m.Add("MessageText", "There are no messages");
                    DefaultMessage.Add(m);
                    ViewBag.Messages = DefaultMessage;
                }
                else {
                    ViewBag.Messages = messages;
                }

                string CommentQuery = $@"SELECT Users.FirstName AS FirstName, Users.LastName AS LastName, Comments.id as id, Comments.UserID AS UserID, Comments.MessageID AS MessageID, Comments.CommentText AS CommentText, DATE_FORMAT(Comments.CreatedAt, '%b %d, %Y') as CreatedAt
                                        FROM Comments
                                        JOIN Users ON Users.id = Comments.UserID
                                        ORDER BY Comments.CreatedAt DESC;";
                List<Dictionary<string,object>>  comments = _dbConnector.Query(CommentQuery);

                if(comments.Count == 0)
                {
                    List<Dictionary<string,string>> DefaultComment = new List<Dictionary<string,string>>();
                    Dictionary<string,string> m = new Dictionary<string,string>();
                    m.Add("CommentText", "");
                    DefaultComment.Add(m);
                    ViewBag.Comments = DefaultComment;
                }
                else {
                    ViewBag.Comments = comments;
                }
                
                return View();
            }
        }

        [HttpGet]
        [Route("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public void RegisterUser(RegisterUser user)
        {
            PasswordHasher<RegisterUser> hasher = new PasswordHasher<RegisterUser>();
            string hashed = hasher.HashPassword(user, user.Password);

            string query = $@"INSERT INTO Users (FirstName, LastName, Email, Password, CreatedAt, UpdatedAt)
                    VALUES ('{user.FirstName}', '{user.LastName}', '{user.Email}', '{hashed}', NOW(), NOW());
                    SELECT LAST_INSERT_ID() as id";
            HttpContext.Session.SetInt32("id", Convert.ToInt32(_dbConnector.Query(query)[0]["id"]));
        }
    }
}
