using CustomerService.DTOs;
using CustomerService.Models;
using CustomerService.Repositories.Interfaces;

namespace CustomerService.Services.Interfaces
{
    public class CarService : ICarService
    {
        private readonly ICarRepository _carRepository;
        private readonly ICustomerRepository _customerRepository;

        public CarService(
            ICarRepository carRepository,
            ICustomerRepository customerRepository)
        {
            _carRepository = carRepository;
            _customerRepository = customerRepository;
        }

        public async Task<List<Car>> GetCustomerCarsAsync(Guid customerId)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId);

            if (customer == null)
                throw new Exception("Customer not found");

            return await _carRepository.GetByCustomerIdAsync(customerId);
        }

        public async Task<Car?> GetCarAsync(
            Guid customerId,
            Guid carId)
        {
            return await _carRepository.GetByIdAsync(
                customerId,
                carId);
        }

        public async Task<Car> AddCarAsync(
            Guid customerId,
            CreateCarDto dto)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId);

            if (customer == null)
                throw new Exception("Customer not found");

            var car = new Car
            {
                CarId = Guid.NewGuid(),
                CustomerId = customerId,
                Brand = dto.Brand,
                Model = dto.Model,
                RegistrationNumber = dto.RegistrationNumber,
                Color = dto.Color,
                CarType = dto.CarType,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _carRepository.AddAsync(car);

            return car;
        }

        public async Task UpdateCarAsync(
            Guid customerId,
            Guid carId,
            UpdateCarDto dto)
        {
            var car = await _carRepository.GetByIdAsync(
                customerId,
                carId);

            if (car == null)
                throw new Exception("Car not found");

            car.Brand = dto.Brand;
            car.Model = dto.Model;
            car.RegistrationNumber = dto.RegistrationNumber;
            car.Color = dto.Color;
            car.CarType = dto.CarType;
            car.UpdatedAt = DateTime.UtcNow;

            await _carRepository.UpdateAsync(car);
        }

        public async Task DeleteCarAsync(
            Guid customerId,
            Guid carId)
        {
            var car = await _carRepository.GetByIdAsync(
                customerId,
                carId);

            if (car == null)
                throw new Exception("Car not found");

            await _carRepository.DeleteAsync(car);
        }
    }
}