using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace blog.ui.Models
{
    public class Comment
    {
        public long ID { get; set; }
        public string Content { get; set; }
        public DateTime DatePublished { get; set; } = DateTime.UtcNow;
        public long PostId { get; set; }
        public CommentType Type { get; set; } = CommentType.Normal;
    }
}
