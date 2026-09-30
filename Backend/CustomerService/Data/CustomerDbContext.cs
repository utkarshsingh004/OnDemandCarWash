using CustomerService.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Data
{
    public class CustomerDbContext : DbContext
    {
        public CustomerDbContext(
            DbContextOptions<CustomerDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Car> Cars { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Customer Primary Key
            modelBuilder.Entity<Customer>()
                .HasKey(c => c.CustomerId);

            // Car Primary Key
            modelBuilder.Entity<Car>()
                .HasKey(c => c.CarId);

            // Customer 1 : N Car
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Cars)
                .WithOne(c => c.Customer)
                .HasForeignKey(c => c.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Customer Email must be unique
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Email)
                .IsUnique();

            // Car Registration Number must be unique
            modelBuilder.Entity<Car>()
                .HasIndex(c => c.RegistrationNumber)
                .IsUnique();
        }
    }
}