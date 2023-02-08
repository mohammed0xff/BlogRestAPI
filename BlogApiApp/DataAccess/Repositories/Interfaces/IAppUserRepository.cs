using Models.Entities;

namespace DataAccess.Repositories.Interfaces
{
    public interface IAppUserRepository : IRepository<AppUser>
    {
        Task<AppUser> UnSuspendByUsername(string username);
        Task<AppUser> SuspendByUsername(string username);
        Task<bool> UsernameAlreadyExists(string newUsername);
        Task ChangeUsername(string Username, string newUsername);
    }
}
