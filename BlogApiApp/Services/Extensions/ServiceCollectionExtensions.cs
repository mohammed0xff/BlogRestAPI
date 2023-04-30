using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using DataAccess.DataContext;
using Microsoft.AspNetCore.Identity;
using Models.Entities;
using Models.Constants;

namespace Services.Extensions
{
    static public class ServiceCollectionExtensions
    {
        public static WebApplicationBuilder AddCustomIdentity(this WebApplicationBuilder builder)
        {
            builder.Services.AddIdentity<AppUser, IdentityRole>(options => {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.SignIn.RequireConfirmedEmail = false;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.User.AllowedUserNameCharacters = UserConstants.AllowedUserNameCharacters;
            })
            .AddEntityFrameworkStores<AppDbContext>();
            
            return builder;
        }
    }
}
