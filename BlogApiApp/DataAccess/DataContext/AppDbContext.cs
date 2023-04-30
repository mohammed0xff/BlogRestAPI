using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace DataAccess.DataContext
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            
            // App user
            builder.Entity<AppUser>()
                .HasMany<Blog>()
                .WithOne(p => p.User)
                .OnDelete(DeleteBehavior.NoAction);

            // Blog
            builder.Entity<Blog>()
                .HasMany(c => c.Posts)
                .WithOne(p => p.Blog)
                .OnDelete(DeleteBehavior.NoAction);
            
            // Post
            builder.Entity<Post>()
                .HasMany(p => p.Comments)
                .WithOne(c => c.Post)
                .OnDelete(DeleteBehavior.NoAction);

            // Comment like junction table
            builder.Entity<CommentLike>()
                .HasOne(x => x.Comment)
                .WithMany(x => x.Likes)
                .HasForeignKey(x => x.CommentId)
                .OnDelete(DeleteBehavior.NoAction);

            // Post like junction table
            builder.Entity<PostLike>()
                .HasOne(x => x.Post)
                .WithMany(x => x.Likes)
                .HasForeignKey(x => x.PostId)
                .OnDelete(DeleteBehavior.NoAction);

            // Follow
            builder.Entity<Follow>()
                .HasOne(x => x.Blog)
                .WithMany(x => x.Follows)
                .HasForeignKey(x => x.BlogId)
                .OnDelete(DeleteBehavior.NoAction);

            // auto includes
            builder.Entity<Blog>().Navigation(x => x.User).AutoInclude();
            builder.Entity<Post>().Navigation(x => x.PostTags).AutoInclude();

            base.OnModelCreating(builder);
        }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CommentLike> CommentLikes { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostLike> PostLikes { get; set; }
        public DbSet<PostTag> PostTags { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Tag> Tags { get; set; }
    }
}
