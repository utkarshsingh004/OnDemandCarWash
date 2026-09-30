using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data
{
    public class AdminDbContext : DbContext
    {
        public AdminDbContext(
            DbContextOptions<AdminDbContext> options)
            : base(options)
        {
        }

        public DbSet<Admin> Admins { get; set; }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Admin>()
                .HasKey(a => a.AdminId);

            modelBuilder.Entity<Admin>()
                .HasIndex(a => a.Email)
                .IsUnique();
        }
    }
}