using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entities
{
    public class CommentLike
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        
        [ForeignKey(nameof(UserId))]
        public virtual AppUser User { get; set; }

        [Required]
        public int CommentId { get; set; }
        
        [ForeignKey(nameof(CommentId))]
        public virtual Comment Comment { get; set; }
    }
}
