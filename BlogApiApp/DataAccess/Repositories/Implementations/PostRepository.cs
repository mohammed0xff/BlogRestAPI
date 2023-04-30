using DataAccess.DataContext;
using DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.ApiModels.ResponseDTO;
using Models.Entities;
using Models.Query;

namespace DataAccess.Repositories.Implementation
{
    public class PostRepository : Repository<Post>, IPostRepository
    {
        private readonly AppDbContext _appContext;

        public PostRepository(AppDbContext appContext) : base(appContext)
        {
            _appContext = appContext;
        }

        public async Task<PaginatedList<Post>> GetPostsAsync(PostFilterParams postParameters)
        {
            var query = dbSet.AsQueryable();
            
            // filter on blog id
            if(postParameters.BlogId !=  null)
                query.Where(x => x.BlogId.Equals(postParameters.BlogId));
            
            // filter on user id
            if (postParameters.UsreId != null)
                query.Where(x => x.UserId.Equals(postParameters.UsreId));
            
            // get with posts with tag if provided
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
                query.OrderByDescending(x => x.LikesCount)
                    .ThenByDescending(x => x.DatePublished);
            }

            // include post owner and likes 
            query.Include(p => p.User)
                .Include(p => p.Likes);
            
            // to page list 
            return await GetPageAsync(
                query, postParameters.PageNumber, postParameters.PageSize
                );
        }

        public async Task<Post> GetOneAsync(int postId, string userId)
        {
            var post = _appContext.Posts
                .Where(p => p.Id == postId)
                .FirstOrDefault();
            
            if(post != null)
            {
                post.IsLiked = post.Likes.Any(like => like.UserId == userId);
            }

            return post;
        }

        public async Task<bool> IsLikedAsync(int postId, string userId)
        {
            return await _appContext.PostLikes
                .AnyAsync(like => like.PostId.Equals(postId) && like.UserId.Equals(userId));
        }

        public async Task<List<AppUser>> GetLikesAsync(int postId)
        {
            var likes = _appContext.PostLikes.Where(like => like.PostId == postId);
            var users = from l in likes
                        join u in _appContext.Users
                        on l.UserId equals u.Id
                        select u;

            return await users
                .ToListAsync();
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
                .Where(like => like.PostId == postId && like.UserId == userId)
                .SingleOrDefaultAsync();
            
            if(like != null)
            {
                _appContext.PostLikes.Remove(like);
            }
        }

        public async Task<Tag?> GetTagByName(string tagName)
        {
            return await _appContext.Tags
                .SingleOrDefaultAsync(x => x.Name.Equals(tagName));
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