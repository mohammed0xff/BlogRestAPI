
using Models.ApiModels.ResponseDTO;

namespace Models.ApiModels
{
    public class CommentResponse
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime DatePublished { get; set; }
        public int LikesCount { get; set; }
        public bool IsLiked { get; set; }
        public int PostId { get; set; }
        public AppUserResponse? User { get; set; } 
    }
}