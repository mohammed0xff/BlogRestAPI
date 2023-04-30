using Microsoft.AspNetCore.Http;
using Models.Constants;

namespace Services.Authentication.Session
{
    public class Session : ISession
    {
        public DateTime Now => DateTime.Now;
        public string UserId { get; } = null!;
        public string Username { get; } = null!;
        public bool IsAuthenticated { get; } = false;
        public bool IsAdmin { get; }
        public Session(IHttpContextAccessor httpContextAccessor)
        {
            var user = httpContextAccessor.HttpContext?.User;
            IsAuthenticated = user.Identity.IsAuthenticated;
            if (IsAuthenticated)
            {
                UserId = user.Claims.Where(x => x.Type == "uid").FirstOrDefault()?.Value;
                Username = user.Claims.Where(x => x.Type == "username").FirstOrDefault()?.Value;
                IsAdmin = user.IsInRole(Roles.Admin);
            }
        }

    }
}