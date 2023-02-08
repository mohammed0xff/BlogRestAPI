using DataAccess.DataContext;
using DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace DataAccess.Repositories.Implementation
{
    public class AppUserRepository : Repository<AppUser>, IAppUserRepository
    {
        private readonly AppDbContext _dbContext;

        public AppUserRepository(AppDbContext appContext) : base(appContext)
        {
            _dbContext = appContext;
        }

        public async Task ChangeUsername(string Username, string newUsername)
        {
            var user =  await _dbContext.Users
                .AsTracking()
                .FirstOrDefaultAsync(x => x.UserName.Equals(Username));
            user.UserName = newUsername;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<AppUser> SuspendByUsername(string username)
        {
            var user = await _dbContext.Users
                .AsTracking()
                .FirstOrDefaultAsync(u => u.UserName.Equals(username));
            if(user != null)
            {
                user.IsSuspended = true;
                await _dbContext.SaveChangesAsync();
            }

            return user;
        }
        
        public async Task<AppUser> UnSuspendByUsername(string username)
        {
            var user = await _dbContext.Users
                .AsTracking()
                .FirstOrDefaultAsync(u => u.UserName.Equals(username));
            if (user != null)
            {
                user.IsSuspended = true;
                await _dbContext.SaveChangesAsync();
            }

            return user;
        }

        public async Task<bool> UsernameAlreadyExists(string newUsername)
        {
            return await _dbContext.Users
                .AnyAsync(x => x.UserName.Equals(newUsername));
        }
    }
}
