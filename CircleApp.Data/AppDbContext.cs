using CircleApp.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CircleApp.Data
{
    public class AppDbContext : IdentityDbContext<User,IdentityRole<int>,int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<Like> Likes { get; set; }

        public DbSet<Comment> Comments { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Story> Stories { get; set; }
        public DbSet<HashTag> Hashtags { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Posts)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId);


            modelBuilder.Entity<User>()
                .HasMany(u => u.Stories)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId);

            modelBuilder.Entity<Like>()
                .HasKey(p => new { p.UserId, p.PostId });

            modelBuilder.Entity<Like>()
                .HasOne(l => l.User)
                .WithMany(p => p.Likes)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Like>()
            .HasOne(l => l.Post)
            .WithMany(p => p.Likes)
            .HasForeignKey(p => p.PostId)
            .OnDelete(DeleteBehavior.Cascade);




            modelBuilder.Entity<Comment>()
               .HasOne(l => l.User)
               .WithMany(p => p.Comments)
               .HasForeignKey(p => p.UserId)
               .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Comment>()
            .HasOne(l => l.Post)
            .WithMany(p => p.Comments)
            .HasForeignKey(p => p.PostId)
            .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Favorite>()
                .HasKey(f => new { f.PostId, f.UserId });


            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Post)
                .WithMany(n => n.Favorites)
                .HasForeignKey(P => P.PostId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithMany(f => f.Favorites)
                .HasForeignKey(P => P.UserId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Report>()
            .HasKey(f => new { f.PostId, f.UserId });


            modelBuilder.Entity<Report>()
                .HasOne(f => f.Post)
                .WithMany(n => n.Reports)
                .HasForeignKey(P => P.PostId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Report>()
                .HasOne(f => f.User)
                .WithMany(f => f.Reports)
                .HasForeignKey(P => P.UserId)
                .OnDelete(DeleteBehavior.Restrict);


            base.OnModelCreating(modelBuilder);

            //customize asp .net identity model table names
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<IdentityRole<int>>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
            modelBuilder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");

        }

    }
}
