using System;
using System.ComponentModel.DataAnnotations;
namespace The_Wall.Models
{
    public class Message : BaseEntity
    {
        public int UserID { get; set; }

        public string MessageText { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }

    public class Comment : BaseEntity
    {
        public int MessageID { get; set; }

        public int UserID { get; set; }

        public string CommentText { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}