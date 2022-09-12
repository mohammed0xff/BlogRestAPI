using DataAccess.DataContext;
using DataAccess.Repositories.Interfaces;
using Models.Entities;

namespace DataAccess.Repositories.Implementation
{
    public class PostRepository : Repository<Post>, IPostRepository
    {
        private readonly AppDbContext _appContext;

        public PostRepository(AppDbContext appContext) : base(appContext)
        {
            _appContext = appContext;
        }

        public IEnumerable<Post> GetAll(int blogId, string userId)
        {
            var posts = _appContext.Posts.Where(p => p.BlogId == blogId).ToList();
            foreach (var post in posts)
            {
                post.IsLiked = IsLiked(post.Id, userId);
                post.LikesCount = GetLikesCount(post.Id);
            }
            return posts;
        }


        public Post Get(int postId, string userId)
        {
            var post = _appContext.Posts.Where(p => p.Id == postId).FirstOrDefault();
            if(post != null)
            {
                post.LikesCount = GetLikesCount(post.Id);
                post.IsLiked= IsLiked(post.Id,userId);
            }
            return post;

        }

        public bool IsLiked(int postId, string userId)
        {
            return _appContext.PostLikes.Count(like => like.PostId == postId && like.UserId == userId) > 0;
        }

        public List<AppUser> GetLikes(int postId)
        {

            var likes = _appContext.PostLikes.Where(like => like.PostId == postId).ToList();
            var users = from l in likes
                        join u in _appContext.Users
                        on l.UserId equals u.Id
                        select u;



            return users.ToList();
        }

        public int GetLikesCount(int postId)
        {
            return _appContext.PostLikes.Count(like => like.PostId == postId);
        }

        public void AddLike(int postId, string userId)
        {
            if (_appContext.PostLikes.Where(l => l.UserId == userId && l.PostId == postId).Any())
                return;
            var like = new PostLike { UserId = userId, PostId = postId};
            _appContext.PostLikes.Add(like);
        }

        public void RemoveLike(int postId, string userId)
        {
         var like = _appContext.PostLikes.Where(like => like.UserId == userId && like.PostId == postId)
                            .FirstOrDefault();
            if(like != null)
            {
                _appContext.PostLikes.Remove(like);
            }
        }

    }
}