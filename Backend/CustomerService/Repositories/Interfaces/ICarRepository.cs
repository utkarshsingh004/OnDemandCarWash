using CustomerService.Models;

namespace CustomerService.Repositories.Interfaces
{
    public interface ICarRepository
    {
        Task<List<Car>> GetByCustomerIdAsync(Guid customerId);

        Task<Car?> GetByIdAsync(Guid customerId, Guid carId);

        Task AddAsync(Car car);

        Task UpdateAsync(Car car);

        Task DeleteAsync(Car car);
    }
}