using WasherService.DTOs;
using WasherService.Models;

namespace WasherService.Services
{
    public interface IWasherService
{
    Task<List<Washer>> GetAllAsync();

    Task<Washer?> GetByIdAsync(Guid id);

    Task<Washer?> GetByEmailAsync(string email);

    Task<Washer> RegisterAsync(RegisterWasherDto dto);

    Task<Washer?> LoginAsync(LoginWasherDto dto);

    Task UpdateAsync(Guid id, UpdateWasherDto dto);

    Task UpdateProfileAsync(Guid id, UpdateWasherProfileDto dto);

    Task UpdateAvailabilityAsync(Guid id, bool isAvailable);

    Task<List<Washer>> GetAvailableAsync();

    Task DeleteAsync(Guid id);
}
}