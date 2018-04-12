using System;
using System.ComponentModel.DataAnnotations;
namespace The_Wall.Models
{
    public class RegisterUser : BaseEntity
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }

    public class LoginUser : BaseEntity
    {
        [Required]
        [Display(Name = "Email")]
        public string LogEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string LogPassword { get; set; }
    }
}