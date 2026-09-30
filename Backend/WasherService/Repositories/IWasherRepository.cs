using WasherService.Models;

namespace WasherService.Repositories
{
    public interface IWasherRepository
    {
        Task<List<Washer>> GetAllAsync();

        Task<Washer?> GetByIdAsync(Guid id);

        Task<Washer?> GetByEmailAsync(string email);

        Task AddAsync(Washer washer);

        Task UpdateAsync(Washer washer);

        Task<List<Washer>> GetAvailableAsync();

        Task DeleteAsync(Washer washer);
    }
}