using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using The_Wall.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Web;

namespace The_Wall.Controllers
{
    public class MessageController : Controller
    {

        private readonly DbConnector _dbConnector;
 
        public MessageController(DbConnector connect)
        {
            _dbConnector = connect;
        }

        [HttpPost]
        [Route("post/write")]
        public IActionResult WriteMessage(MessageViewModel model)
        {
            if(ModelState.IsValid)
            {
                Message NewMessage = new Message
                {
                    UserID = (int)HttpContext.Session.GetInt32("id"),
                    MessageText = System.Security.SecurityElement.Escape(model.MessageText)
                };
                WriteMessage(NewMessage);
            }

            return RedirectToAction("Dashboard", "Home");
        }

        [HttpPost]
        [Route("post/{id}/writecomment")]
        public IActionResult WriteComment(int id, CommentViewModel model)
        {

            if(ModelState.IsValid)
            {
                Comment NewComment = new Comment
                {
                    UserID = (int)HttpContext.Session.GetInt32("id"),
                    MessageID = (int)id,
                    CommentText = System.Security.SecurityElement.Escape(model.CommentText)
                };
                WriteComment(NewComment);
            }

            return RedirectToAction("Dashboard", "Home");
        }

        [HttpGet]
        [Route("post/{id}/delete")]
        public IActionResult Delete(int id)
        {
            string query = $"SELECT * FROM Messages WHERE id={id};";
            List<Dictionary<string,object>> Message = _dbConnector.Query(query);
            ViewBag.Message = Message;

            return View();
        }

        [HttpGet]
        [Route("post/{id}/destroy")]
        public IActionResult Destroy(int id)
        {
            string query = $"DELETE FROM Messages WHERE id={id};";
            _dbConnector.Execute(query);
            return RedirectToAction("Dashboard", "Home");
        }

        public void WriteMessage(Message message)
        {
            string query = $@"INSERT INTO Messages (UserID, MessageText, CreatedAt, UpdatedAt) 
                            VALUES ('{message.UserID}','{message.MessageText}', NOW(), NOW());";
            _dbConnector.Execute(query);

        }

        public void WriteComment(Comment comment)
        {
            string query = $@"INSERT INTO Messages (MessageID, UserID, CommentText, CreatedAt, UpdatedAt) 
                            VALUES ('{comment.MessageID}', {comment.UserID}','{comment.CommentText}', NOW(), NOW());";
            _dbConnector.Execute(query);

        }
    }
}