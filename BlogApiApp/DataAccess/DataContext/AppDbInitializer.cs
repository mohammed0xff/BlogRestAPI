using DataAccess.DataContext;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Models.Entities;



namespace DataAccess
{
    public class AppDbInitializer
    {

        public static  void SeedData(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
                context.Database.EnsureCreated();


                #region BlogSeed
                if (!context.Blogs.Any())
                {
                    var john = context.Users.First(x => x.FirstName == "john");
                    var jane = context.Users.First(x => x.FirstName == "jane");

                    context.Blogs.AddRange(
                        new Blog
                        {
                            UserId = john.Id,
                            Title = "First blog",
                            Description = "Welcome to my blog :)",
                            Posts = new List<Post>
                            {
                                new Post()
                                {
                                    UserId = john.Id,
                                    HeadLine = "first post",
                                    Content = "",
                                    Comments = new List<Comment>
                                    {
                                        new Comment()
                                        {
                                            UserId = jane.Id,
                                            Content = "nice post",
                                        }
                                    }
                                }
                            },
                        },

                        new Blog
                        {
                            UserId = jane.Id,
                            Title = "second blog",
                            Description = "Welcome to my blog :)",
                            Posts = new List<Post>
                            {
                                new Post()
                                {
                                    UserId = jane.Id,
                                    HeadLine = "Blessings.",
                                    Content = "",
                                    Comments = new List<Comment>
                                    {
                                        new Comment()
                                        {
                                            UserId = john.Id,
                                            Content = "Hello!",
                                        }
                                    }
                                }
                            },
                        },


                        new Blog
                        {
                            UserId = john.Id,
                            Title = "third blog",
                            Description = "Welcome to my blog :)",
                            Posts = new List<Post>
                            {
                                new Post()
                                {
                                    UserId = john.Id,
                                    HeadLine = "",
                                    Content = "",
                                    Comments = new List<Comment>
                                    {
                                        new Comment()
                                        {
                                            UserId = john.Id,
                                            Content = "",

                                        }
                                    }
                                }
                            },
                        }

                        );
                }

                #endregion

                context.SaveChanges();
               
            }

        }

        public static async Task SeedUsersAndRolesAsync(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                //Roles
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
                if (!await roleManager.RoleExistsAsync(UserRoles.User))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

                var dbcontext = serviceScope.ServiceProvider.GetService<AppDbContext>();
                var userManager = serviceScope.ServiceProvider.GetService<UserManager<AppUser>>();

                #region UsersSeed
                if (!dbcontext.Users.Any())
                {
                    var user1 =
                        new AppUser
                        {
                            FirstName = "john",
                            LastName = "doe",
                            Email = "johndoe@gmail.com",
                            UserName = "john123",

                        };

                    var user2 =
                        new AppUser
                        {
                            FirstName = "jane",
                            LastName = "doe",
                            Email = "janedoe@gmail.com",
                            UserName = "jane123",
                                
                        };

                    await userManager.CreateAsync(user1, "Passwd@123");
                    await userManager.AddToRoleAsync(user1, UserRoles.User);

                    await userManager.CreateAsync(user2, "Passwd@123");
                    await userManager.AddToRoleAsync(user2, UserRoles.User);

                }
                #endregion

            }
        }
    }
}
