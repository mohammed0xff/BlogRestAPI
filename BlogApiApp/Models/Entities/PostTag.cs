using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entities
{
    public class PostTag
    {
        public int Id { get; set; }

        [ForeignKey(nameof(TagId))]
        public virtual Tag Tag { get; set; }
        public int TagId { get; set; }
        
        [ForeignKey(nameof(PostId))]
        public virtual Post post { get; set; }
        public int PostId { get; set; }
    }
}
