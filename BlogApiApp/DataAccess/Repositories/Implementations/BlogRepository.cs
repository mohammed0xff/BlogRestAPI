using DataAccess.DataContext;
using DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.ApiModels.ResponseDTO;
using Models.Entities;
using Models.Query;

namespace DataAccess.Repositories.Implementation
{
    internal class BlogRepository : Repository<Blog>, IBlogRepository
    {
        private readonly AppDbContext _appContext;

        public BlogRepository(AppDbContext appContext) : base(appContext)
        {
            _appContext = appContext;
        }

        public Task<PaginatedList<Blog>> GetBlogsAsync(BlogFilterParams blogParameters)
        {
            var query = dbSet.AsQueryable();
            if (!string.IsNullOrEmpty(blogParameters.Username))
            {
                query.Where(x => x.User.UserName == blogParameters.Username);
            }
            if (blogParameters.Popular)
            {
                query.OrderByDescending(x => x.FollowersCount);
            }

            return GetPageAsync(query, blogParameters.PageNumber, blogParameters.PageSize);
        }

        public async Task<Blog?> GetOneByIdAsync(int BlogId)
        {
            return await dbSet.Where(b => b.Id == BlogId)
                .SingleOrDefaultAsync();
        }

        public async Task<List<Blog>> GetFollowedBlogsAsync(string userId)
        {
            var follows = _appContext.Follows
                .Where(b => b.UserId == userId);
            
            var followedBloges = from b in _appContext.Blogs
                                 join f in follows
                                 on b.Id equals f.BlogId
                                 select b;

            return await followedBloges.ToListAsync();
        }

        public async Task<List<Blog>> GetBlogsByUserIdAsync(string userId)
        {
            return await _appContext.Blogs
                .Where(b => b.UserId.Equals(userId))
                .ToListAsync();
        }

        public async Task<List<AppUser>> GetFollowers(int blogid)
        {
            var followers = _appContext.Follows
                .Where(f => f.BlogId.Equals(blogid));

            var res = _appContext.Users.Join(followers,
                    user => user.Id,
                    follow => follow.UserId,
                    (u, f) => u
                );

            return await res.ToListAsync();
        }

        public async Task AddFollowerAsync(int blogId, string userId)
        {
            if (!_appContext.Follows.Any(x => x.BlogId == blogId && x.UserId == userId))
            {
                var blog = await _appContext.Blogs.Where(x => x.Id == blogId)
                    .AsTracking()
                    .FirstOrDefaultAsync();

                blog!.FollowersCount++;

                await _appContext.Follows
                    .AddAsync(new Follow { 
                        BlogId = blogId, UserId = userId 
                    });
            }
        }

        public async Task RemoveFollowerAsync(int blogId, string userId)
        {
            var follow = await _appContext.Follows
                .Where(b => b.UserId == userId && b.BlogId == blogId)
                .SingleOrDefaultAsync();
            
            if (follow != null)
            {
                var blog = await _appContext.Blogs.Where(x => x.Id == blogId)
                    .AsTracking()
                    .FirstOrDefaultAsync();
                
                blog!.FollowersCount--;
                
                _appContext.Follows.Remove(follow);
            }
        }
    }
}