using blog.api.Events;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace blog.common.Model
{
    public class PostDB : BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Content { get; set; } = string.Empty;
        public DateTime? DatePublished { get; set; } = null;
        public long OwnerId { get; set; }
        public PostStatus Status { get; set; } = PostStatus.None;
        public List<CommentDB> Comments { get; set; } = new List<CommentDB>();
    }
}
