using DataAccess.DataContext;
using DataAccess.Repositories.Interfaces;
using Models.Entities;

namespace DataAccess.Repositories.Implementation
{
    internal class BlogRepository : Repository<Blog>, IBlogRepository
    {
        private readonly AppDbContext _appContext;

        public BlogRepository(AppDbContext appContext) : base(appContext)
        {
            _appContext = appContext;
        }

        public List<Blog> GetFollowedBlogs(string userId)
        {
            var follows = _appContext.Follows.Where(b => b.UserId == userId);
            var followedBloges = from b in _appContext.Blogs
                                 join f in follows
                                 on b.Id equals f.BlogId
                                 select b;
            return followedBloges.ToList();
        }

        public void AddFollower(int blogId, string userId)
        {
            if(!_appContext.Follows.Any(x => x.BlogId == blogId && x.UserId == userId))
            {
                var follow = new Follow { BlogId = blogId, UserId = userId };
                _appContext.Follows.Add(follow);
            }
        }

        public void RemoveFollower(int blogId, string userId)
        {
            var follow = _appContext.Follows.Where(b => b.UserId == userId && b.BlogId == blogId).FirstOrDefault();
            if(follow != null)
            {
                _appContext.Follows.Remove(follow);
            }
        }

        public int GetFollowersCount(int blogId)
        {
            return _appContext.Follows.Count(b => b.Id == blogId);
        }
    }
}