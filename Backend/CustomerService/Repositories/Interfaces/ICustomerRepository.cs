using CustomerService.Models;

namespace CustomerService.Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        Task<List<Customer>> GetAllAsync();

        Task<Customer?> GetByIdAsync(Guid id);

        Task<Customer?> GetByEmailAsync(string email);

        Task AddAsync(Customer customer);

        Task UpdateAsync(Customer customer);

        Task DeleteAsync(Customer customer);
    }
}