
namespace DataAccess.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        IAppUserRepository AppUsers { get; }
        IBlogRepository BlogRepository { get; }
        IPostRepository PostRepository { get; }
        ICommentRepository CommentRepository { get; }
        ITokenRepository TokenRepository { get; }

        Task SaveAsync();
    }
}
