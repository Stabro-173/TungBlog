using Microsoft.EntityFrameworkCore;
using TungBlog.Models;
using TungBlog.Services;

namespace TungBlog.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<Article> Articles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed admin account
            modelBuilder.Entity<UserAccount>().HasData(
                new UserAccount
                {
                    Id = 999999,
                    Username = "admin",
                    Password = PasswordHasher.HashPassword("admin"),
                    FullName = "Administrator",
                    Email = "admin@tungblog.com",
                    Role = "Admin",
                    DateOfBirth = new DateTime(2000, 1, 1)
                }
            );
        }
    }
}
