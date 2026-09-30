using Microsoft.AspNetCore.Identity;
using WasherService.DTOs;
using WasherService.Models;
using WasherService.Repositories;

namespace WasherService.Services
{
    public class WasherService : IWasherService
    {
        private readonly IWasherRepository _repository;
        private readonly PasswordHasher<Washer> _passwordHasher;

        public WasherService(IWasherRepository repository)
        {
            _repository = repository;
            _passwordHasher = new PasswordHasher<Washer>();
        }

        public async Task<List<Washer>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Washer?> GetByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Washer> RegisterAsync(RegisterWasherDto dto)
        {
            var existingWasher =
                await _repository.GetByEmailAsync(dto.Email);

            if (existingWasher != null)
                throw new Exception("Email already registered");

            var washer = new Washer
            {
                WasherId = Guid.NewGuid(),
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                IsAvailable = true,
                Priority = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            washer.PasswordHash =
                _passwordHasher.HashPassword(
                    washer,
                    dto.Password
                );

            await _repository.AddAsync(washer);

            return washer;
        }

        public async Task<Washer?> LoginAsync(LoginWasherDto dto)
        {
            var washer =
                await _repository.GetByEmailAsync(dto.Email);

            if (washer == null)
                return null;

            var result =
                _passwordHasher.VerifyHashedPassword(
                    washer,
                    washer.PasswordHash,
                    dto.Password
                );

            if (result == PasswordVerificationResult.Failed)
                return null;

            return washer;
        }

        public async Task UpdateAsync(
            Guid id,
            UpdateWasherDto dto)
        {
            var washer =
                await _repository.GetByIdAsync(id);

            if (washer == null)
                throw new Exception("Washer not found");

            washer.Name = dto.Name;
            washer.Email = dto.Email;
            washer.Phone = dto.Phone;
            washer.Address = dto.Address;
            washer.IsAvailable = dto.IsAvailable;
            washer.Priority = dto.Priority;
            washer.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(washer);
        }

        public async Task UpdateAvailabilityAsync(
    Guid id,
    bool isAvailable)
        {
            var washer =
                await _repository.GetByIdAsync(id);

            if (washer == null)
                throw new Exception("Washer not found");

            washer.IsAvailable = isAvailable;
            washer.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(washer);
        }

        public async Task<List<Washer>> GetAvailableAsync()
        {
            return await _repository.GetAvailableAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var washer =
                await _repository.GetByIdAsync(id);

            if (washer == null)
                throw new Exception("Washer not found");

            await _repository.DeleteAsync(washer);
        }
    }
}