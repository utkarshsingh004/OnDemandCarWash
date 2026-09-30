using CustomerService.DTOs;
using CustomerService.Models;

namespace CustomerService.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<List<Customer>> GetAllAsync();

        Task<Customer?> GetByIdAsync(Guid id);

        Task<Customer> RegisterAsync(RegisterCustomerDto dto);

        Task<Customer?> LoginAsync(LoginCustomerDto dto);

        Task UpdateAsync(Guid id, UpdateCustomerDto dto);

        Task DeleteAsync(Guid id);
    }
}