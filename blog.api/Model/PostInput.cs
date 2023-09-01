using System.ComponentModel.DataAnnotations;

namespace blog.common.Model
{
    public class PostInput
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Content { get; set; } = string.Empty;
    }
}
