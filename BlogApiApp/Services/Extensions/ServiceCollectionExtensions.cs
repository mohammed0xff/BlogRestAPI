using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Hellang.Middleware.ProblemDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using DataAccess.DataContext;
using Microsoft.AspNetCore.Identity;
using Models.Entities;
using Models.Constants;

namespace Services.Extensions
{
    static public class ServiceCollectionExtensions
    {
        public static WebApplicationBuilder AddCustomProblemDetails(this WebApplicationBuilder builder)
        {
            AddCustomProblemDetails(builder.Services);

            return builder;
        }
        public static IServiceCollection AddCustomProblemDetails(this IServiceCollection services)
        {
            services.AddProblemDetails(x =>
            {
                x.ShouldLogUnhandledException = (httpContext, exception, problemDetails) =>
                {
                    var env = httpContext.RequestServices.GetRequiredService<IHostEnvironment>();
                    return env.IsDevelopment() || env.IsStaging();
                };
                x.IncludeExceptionDetails = (ctx, _) =>
                {
                    // Fetch services from HttpContext.RequestServices
                    var env = ctx.RequestServices.GetRequiredService<IHostEnvironment>();
                    return env.IsDevelopment() || env.IsStaging();
                };

                // map exceptions to problems 


            });

            return services;
        }

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
