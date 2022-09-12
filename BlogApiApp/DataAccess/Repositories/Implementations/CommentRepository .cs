using DataAccess.DataContext;
using DataAccess.Repositories.Interfaces;
using Models.Entities;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Implementation
{
    public class CommmentRepository : Repository<Comment>, ICommentRepository
    {
        private readonly AppDbContext _appContext;

        public CommmentRepository(AppDbContext appContext) : base(appContext)
        {
            _appContext = appContext;
        }

        public IEnumerable<Comment> GetAll(int postId, string userId)
        {
            var commments = _appContext.Comments
                .Where(c => c.PostId == postId)
                .Include(x => x.User)
                .ToList();

            foreach (var comment in commments)
            {
                var commentLikes = _appContext.CommentLikes
                    .Where(like => like.CommentId == comment.Id)
                    .ToList();

                comment.IsLiked = commentLikes.Any(like => like.UserId == userId);
                comment.LikesCount = commentLikes.Count();
            }
            return commments;
        }


        public void AddLike(int commentId, string userId)
        {
            _appContext.CommentLikes.Add(
                new CommentLike { UserId = userId, CommentId = commentId }
                );
        }

        public void RemoveLike(int commentId, string userId)
        {
            var like = _appContext.CommentLikes
                .Where(like => like.UserId == userId && like.CommentId == commentId)
                .FirstOrDefault();

            if (like != null)
            {
                _appContext.CommentLikes.Remove(like);
            }
        }

        public int GetLikesCount(int commentId)
        {
            return _appContext.CommentLikes
                .Count(like => like.CommentId == commentId);
        }


        public bool IsLiked(int commentId, string userId)
        {
            return _appContext.CommentLikes
                .Count(like => like.CommentId == commentId && like.UserId == userId) > 0;
        }

        
    }
}