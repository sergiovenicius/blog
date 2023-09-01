using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace blog.common.Model
{
    public class CommentDB
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public DateTime DatePublished { get; set; } = DateTime.UtcNow;
        [Required]
        public long PostId { get; set; }
        public CommentType Type { get; set; } = CommentType.Normal;
    }
}
