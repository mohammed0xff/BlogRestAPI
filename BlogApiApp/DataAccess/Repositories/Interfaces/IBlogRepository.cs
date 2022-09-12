using Models.Entities;

namespace DataAccess.Repositories.Interfaces
{
    public interface IBlogRepository : IRepository<Blog>
    {
        void AddFollower(int blogId, string userId);
        void RemoveFollower(int blogId, string userId);
        List<Blog> GetFollowedBlogs(string userId);
        int GetFollowersCount(int blogId);

    }
}