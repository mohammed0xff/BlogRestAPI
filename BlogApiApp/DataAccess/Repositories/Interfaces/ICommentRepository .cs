using Models.Entities;

namespace DataAccess.Repositories.Interfaces
{
    public interface ICommentRepository : IRepository<Comment>
    {
        Task<IEnumerable<Comment>> GetAllCommentstAsync(int postId, string userId);
        Task<int> GetLikesCount(int commentId);
        Task AddLikeAsync(int CommentId, string userId);
        Task RemoveLikeAsync(int CommentId, string userId);
        Task<bool> IsLiked(int commentId, string userId);

    }
}