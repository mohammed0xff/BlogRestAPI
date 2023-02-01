using Models.ApiModels.ResponseDTO;
using Models.Entities;

namespace DataAccess.Repositories.Interfaces
{
    public interface IBlogRepository : IRepository<Blog>
    {
        Task<PagedList<Blog>> GetPageAsync(int pageNumber, int pageSize, string? userId);
        void AddFollower(int blogId, string userId);
        void RemoveFollower(int blogId, string userId);
        Task<List<Blog>> GetFollowedBlogsAsync(string userId);
        Task<List<Blog>> GetBlogsByUserId(string userId);

    }
}