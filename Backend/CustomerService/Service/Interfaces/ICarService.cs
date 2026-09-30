using CustomerService.DTOs;
using CustomerService.Models;

namespace CustomerService.Services.Interfaces
{
    public interface ICarService
    {
        Task<List<Car>> GetCustomerCarsAsync(Guid customerId);

        Task<Car?> GetCarAsync(
            Guid customerId,
            Guid carId);

        Task<Car> AddCarAsync(
            Guid customerId,
            CreateCarDto dto);

        Task UpdateCarAsync(
            Guid customerId,
            Guid carId,
            UpdateCarDto dto);

        Task DeleteCarAsync(
            Guid customerId,
            Guid carId);
    }
}