using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Models.Entities
{
    public class Post
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [MaxLength(50)]
        [Required]
        public string HeadLine { get; set; }

        [MaxLength(500)]
        public string Content { get; set; }

        public DateTime DatePublished { get; set; }
        public bool CommentsDisabled { get; set; } = true;

        [Required]
        public int BlogId { get; set; }

        [ForeignKey(nameof(BlogId))]
        public virtual Blog Blog { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual AppUser User { get; set; }

        [NotMapped]
        public int LikesCount { get; set; }

        [NotMapped]
        public bool IsLiked { get; set; }

        public ICollection<PostLike> Likes { get; set; }

        public ICollection<Comment> Comments { get; set; }

    }
}
