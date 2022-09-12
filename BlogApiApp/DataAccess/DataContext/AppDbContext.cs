using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace DataAccess.DataContext
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base(options)
        {}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<AppUser>()
                .HasMany<Blog>()
                .WithOne(p => p.User)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Blog>()
                .HasMany(c => c.Posts)
                .WithOne(p => p.Blog)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Post>()
                .HasMany(p => p.Comments) 
                .WithOne(c => c.Post)
                .OnDelete(DeleteBehavior.NoAction);



            builder.Entity<CommentLike>()
                .HasOne(x => x.Comment)
                .WithMany(x => x.Likes)
                .HasForeignKey(x => x.CommentId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<PostLike>()
                .HasOne(x => x.Post)
                .WithMany(x => x.Likes)
                .HasForeignKey(x => x.PostId)
                .OnDelete(DeleteBehavior.NoAction);
            
            builder.Entity<Follow>()
                .HasOne(x => x.Blog)
                .WithMany(x => x.Follows)
                .HasForeignKey(x=> x.BlogId)
                .OnDelete(DeleteBehavior.NoAction);



            builder.Entity<Comment>()
                .Property(p => p.DatePublished)
                .HasComputedColumnSql("GETDATE()");

            builder.Entity<Post>()
                .Property(p => p.DatePublished)
                .HasComputedColumnSql("GETDATE()");

            

            base.OnModelCreating(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<AppUser> Users{ get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Follow> Follows { get; set; }  
        public DbSet<PostLike> PostLikes { get; set; }
        public DbSet<CommentLike> CommentLikes { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

    }
}
