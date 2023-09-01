using System.ComponentModel.DataAnnotations;

namespace blog.common.Model
{
    public class CommentInput
    {
        [Required]
        public string Content { get; set; }
    }
}
