using DataAccess.DataContext;
using DataAccess.Repositories.Interfaces;
using Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DataAccess.Repositories.Implementation
{
    internal class BlogRepository : Repository<Blog>, IBlogRepository
    {
        private readonly AppDbContext _appContext;

        public BlogRepository(AppDbContext appContext) : base(appContext)
        {
            _appContext = appContext;
        }

        public async Task<List<Blog>> GetFollowedBlogsAsync(string userId)
        {
            var follows = _appContext.Follows.Where(b => b.UserId == userId);
            var users = _appContext.Users.ToList().AsQueryable();
            var followedBloges = from b in _appContext.Blogs
                                 join f in follows
                                 on b.Id equals f.BlogId
                                 select b;
            return await followedBloges.ToListAsync();
        }

        public async Task<List<Blog>> GetBlogsByUserId(string userId)
        {
            return await _appContext.Blogs
                .Where(b => b.UserId.Equals(userId))
                .ToListAsync();
        }

        public void AddFollower(int blogId, string userId)
        {
            if(!_appContext.Follows.Any(x => x.BlogId == blogId && x.UserId == userId))
            {
                var blog = _appContext.Blogs.Where(x => x.Id == blogId).FirstOrDefault();
                blog!.FollowersCount++;
                var follow = new Follow { BlogId = blogId, UserId = userId };
                _appContext.Follows.Add(follow);
            }
        }

        public void RemoveFollower(int blogId, string userId)
        {
            var follow = _appContext.Follows.Where(b => b.UserId == userId && b.BlogId == blogId).FirstOrDefault();
            if(follow != null)
            {
                var blog = _appContext.Blogs.Where(x => x.Id == blogId).FirstOrDefault();
                blog!.FollowersCount--;
                _appContext.Follows.Remove(follow);
            }
        }

    }
}