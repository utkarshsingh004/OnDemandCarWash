using AuthService.Data;
using AuthService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Services
{
    public static class AdminSeeder
    {
        public static async Task SeedAsync(
            AdminDbContext context)
        {
            if (await context.Admins.AnyAsync())
                return;

            var admin = new Admin
            {
                AdminId = Guid.NewGuid(),
                Name = "System Admin",
                Email = "admin@carwash.com",
                Role = "Admin",
                CreatedAt = DateTime.UtcNow
            };

            var passwordHasher = new PasswordHasher<Admin>();

            admin.PasswordHash =
                passwordHasher.HashPassword(
                    admin,
                    "Admin@123");

            context.Admins.Add(admin);

            await context.SaveChangesAsync();
        }
    }
}