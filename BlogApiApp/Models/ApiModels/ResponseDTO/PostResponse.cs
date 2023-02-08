
using Models.ApiModels.ResponseDTO;
using Models.Entities;

namespace Models.ApiModels
{
    public class PostResponse
    {
        public int Id { get; set; }
        public int BlogId { get; set; }
        public string HeadLine { get; set; }
        public string Content { get; set; }
        public bool CommentsAllowed { get; set; }
        public int LikesCount { get; set; }
        public DateTime DatePublished { get; set; }
        public ICollection<TagResponse> Tags { get; set; }

    }
}
