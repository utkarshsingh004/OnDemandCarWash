using Microsoft.EntityFrameworkCore;
using WasherService.Models;

namespace WasherService.Data
{
    public class WasherDbContext : DbContext
    {
        public WasherDbContext(
            DbContextOptions<WasherDbContext> options)
            : base(options)
        {
        }

        public DbSet<Washer> Washers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Washer>()
                .HasKey(w => w.WasherId);

            modelBuilder.Entity<Washer>()
                .HasIndex(w => w.Email)
                .IsUnique();
        }
    }
}