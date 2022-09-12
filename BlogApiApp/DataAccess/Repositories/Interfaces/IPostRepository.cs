using Models.Entities;

namespace DataAccess.Repositories.Interfaces
{
    public interface IPostRepository : IRepository<Post>
    {
        IEnumerable<Post> GetAll(int blogId, string userId);
        Post Get(int postId, string userId);
        void AddLike(int postId, string userId);
        void RemoveLike(int postId, string userId);
        List<AppUser> GetLikes(int postId);
        int GetLikesCount(int postId);
    }
}