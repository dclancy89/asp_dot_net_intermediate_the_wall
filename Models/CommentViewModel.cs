using System.ComponentModel.DataAnnotations;

namespace The_Wall.Models
{
    public class CommentViewModel : BaseEntity
    {
        [Required]
        [Display(Name = "Post a Comment")]
        public string CommentText { get; set; }
    }

}