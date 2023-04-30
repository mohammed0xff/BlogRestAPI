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
        public bool CommentsDisabled { get; set; } = false;

        [Required]
        public int BlogId { get; set; }

        [ForeignKey(nameof(BlogId))]
        public virtual Blog Blog { get; set; }

        [Required]
        public string UserId { get; set; }

        [NotMapped]
        public int LikesCount => Likes.Count();

        [NotMapped]
        public bool IsLiked { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual AppUser User { get; set; }

        public ICollection<PostLike> Likes { get; set; }

        public ICollection<Comment> Comments { get; set; }
        public ICollection<PostTag> PostTags { get; set; }


    }
}
