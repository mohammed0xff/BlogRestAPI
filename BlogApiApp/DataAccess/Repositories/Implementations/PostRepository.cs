using DataAccess.DataContext;
using DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.ApiModels.ResponseDTO;
using Models.Entities;
using Models.Query;
using System.Security.Cryptography.X509Certificates;

namespace DataAccess.Repositories.Implementation
{
    public class PostRepository : Repository<Post>, IPostRepository
    {
        private readonly AppDbContext _appContext;

        public PostRepository(AppDbContext appContext) : base(appContext)
        {
            _appContext = appContext;
        }

        public async Task<PagedList<Post>> GetPostsAsync(PostParameters postParameters)
        {
            IQueryable<Post> query = dbSet.AsQueryable();
            if(postParameters.BlogId!= null)
                query.Where(x => x.BlogId.Equals(postParameters.BlogId));
            if (postParameters.UsreId != null)
                query.Where(x => x.UserId.Equals(postParameters.UsreId));
            if (!string.IsNullOrEmpty(postParameters.Tag))
            {
                var tag = GetTagByName(postParameters.Tag);
                if (tag != null)
                {
                    var postTags = _appContext.PostTags
                        .Where(x => x.TagId == tag.Id);
                    query.Join(postTags, 
                            p => p.Id, 
                            t => t.PostId, 
                            (p, t) => p
                        );
                }
            }
            if (postParameters.MostLiked)
            {
                query.OrderByDescending(x => x.LikesCount);
            }
            var posts = await GetPageAsync(query, postParameters.PageNumber, postParameters.PageSize);
            
            foreach (var post in posts)
            {
                post.IsLiked = await IsLikedAsync(post.Id, postParameters.UsreId);
                post.LikesCount = await GetLikesCountAsync(post.Id);
            }
            
            return posts;
        }

        public async Task<Post> GetOneAsync(int postId, string userId)
        {
            var post = _appContext.Posts.Where(p => p.Id == postId).FirstOrDefault();
            if(post != null)
            {
                post.LikesCount = await GetLikesCountAsync(post.Id);
                post.IsLiked= await IsLikedAsync(post.Id,userId);
            }
            return post;
        }

        public async Task<bool> IsLikedAsync(int postId, string userId)
        {
            return await _appContext.PostLikes.AnyAsync(like => like.PostId == postId && like.UserId == userId);
        }

        public async Task<List<AppUser>> GetLikesAsync(int postId)
        {

            var likes = _appContext.PostLikes.Where(like => like.PostId == postId).ToList();
            var users = from l in likes
                        join u in _appContext.Users
                        on l.UserId equals u.Id
                        select u;

            return await users
                .AsQueryable()
                .ToListAsync();
        }

        public async Task<int> GetLikesCountAsync(int postId)
        {
            return await _appContext.PostLikes.CountAsync(like => like.PostId == postId);
        }

        public async Task AddLikeAsync(int postId, string userId)
        {
            if (_appContext.PostLikes.Where(l => l.UserId == userId && l.PostId == postId).Any())
                return;
            var like = new PostLike { UserId = userId, PostId = postId};
            await _appContext.PostLikes.AddAsync(like);
        }

        public async Task RemoveLikeAsync(int postId, string userId)
        {
         var like = await _appContext.PostLikes
                .Where(like => like.UserId == userId && like.PostId == postId)
                .FirstOrDefaultAsync();
            if(like != null)
            {
                _appContext.PostLikes.Remove(like);
            }
        }

        public async Task<Tag?> GetTagByName(string tagName)
        {
            return await _appContext.Tags
                .FirstOrDefaultAsync(x => x.Name.Equals(tagName));
        }

        public async Task CreateTag(string tagname)
        {
            await _appContext.Tags.AddAsync(new Tag { Name = tagname });
            await _appContext.SaveChangesAsync();
        }

        public async Task<List<Tag>> GetAvailableTags()
        {
            return await _appContext.Tags
                .ToListAsync();
        }
    }
}