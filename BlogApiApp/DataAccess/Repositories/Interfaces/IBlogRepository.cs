using Models.ApiModels.ResponseDTO;
using Models.Entities;
using Models.Query;

namespace DataAccess.Repositories.Interfaces
{
    public interface IBlogRepository : IRepository<Blog>
    {
        Task<PagedList<Blog>> GetBlogsAsync(BlogParameters blogParameters);
        Task<Blog> GetOneByIdAsync(int BlogId);
        Task<List<Blog>> GetBlogsByUserIdAsync(string userId);
        Task<List<Blog>> GetFollowedBlogsAsync(string userId);
        Task AddFollowerAsync(int blogId, string userId);
        Task RemoveFollowerAsync(int blogId, string userId);
        Task<List<AppUser>> GetFollowers(int blogid);
    }
}