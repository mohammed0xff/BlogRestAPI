using DataAccess.DataContext;
using DataAccess.Repositories.Interfaces;

namespace DataAccess.Repositories.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;

        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            AppUsers = new AppUserRepository(_dbContext);
            BlogRepository = new BlogRepository(_dbContext);
            PostRepository = new PostRepository(_dbContext);
            CommentRepository = new CommentRepository(_dbContext);
            TokenRepository = new TokenRepository(_dbContext);

        }
        public IAppUserRepository AppUsers { get; private set; }
        public IBlogRepository BlogRepository { get; }
        public IPostRepository PostRepository { get; }
        public ICommentRepository CommentRepository { get; }
        public ITokenRepository TokenRepository { get; }

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
