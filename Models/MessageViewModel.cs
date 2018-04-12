using System.ComponentModel.DataAnnotations;

namespace The_Wall.Models
{
    public class MessageViewModel : BaseEntity
    {
        [Required]
        [Display(Name = "Post a Message")]
        public string MessageText { get; set; }
    }

}