using Models.Entities;

namespace DataAccess.Repositories.Interfaces
{
    public interface ICommentRepository : IRepository<Comment>
    {
        void AddLike(int CommentId, string userId);
        void RemoveLike(int CommentId, string userId);
        int GetLikesCount(int commentId);
        bool IsLiked(int commentId, string userId);
        IEnumerable<Comment> GetAll(int postId, string userId);

    }
}