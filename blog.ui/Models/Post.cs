using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace blog.ui.Models
{
    public class Post
    {
        public long ID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime? DatePublished { get; set; } = null;
        public long OwnerId { get; set; }
        public PostStatus Status { get; set; } = PostStatus.None;
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public User Owner { get; set; }
    }
}
