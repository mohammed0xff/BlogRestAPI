using Models.ApiModels.ResponseDTO;
using Models.Entities;
using Models.Query;

namespace DataAccess.Repositories.Interfaces
{
    public interface IPostRepository : IRepository<Post>
    {
        Task<PagedList<Post>> GetPostsAsync(PostParameters postParameters);
        Task<Post> GetOneAsync(int postId, string userId);
        Task AddLikeAsync(int postId, string userId);
        Task RemoveLikeAsync(int postId, string userId);
        Task<int> GetLikesCountAsync(int postId);
        Task<List<AppUser>> GetLikesAsync(int postId);
        Task<Tag?> GetTagByName(string tagName);
        Task CreateTag(string tagname);
        Task<List<Tag>> GetAvailableTags();
    }
}