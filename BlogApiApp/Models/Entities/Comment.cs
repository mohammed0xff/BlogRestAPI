using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entities
{
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(200)]
        public string Content { get; set; }
        public DateTime DatePublished { get; set; } = DateTime.Now;

        [Required]
        public int PostId { get; set; }
        
        [ForeignKey(nameof(PostId))]
        public virtual Post Post { get; set; }

        [Required]
        public string UserId { get; set; }
        
        [ForeignKey(nameof(UserId))]
        public virtual AppUser User { get; set; }
        
        [NotMapped]
        public int LikesCount { get; set; }
        
        [NotMapped]
        public bool IsLiked { get; set; }

        public ICollection<CommentLike> Likes { get; set; }

    }
}