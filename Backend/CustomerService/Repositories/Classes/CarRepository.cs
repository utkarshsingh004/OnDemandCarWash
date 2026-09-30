using CustomerService.Data;
using CustomerService.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Repositories.Interfaces
{
    public class CarRepository : ICarRepository
    {
        private readonly CustomerDbContext _context;

        public CarRepository(CustomerDbContext context)
        {
            _context = context;
        }

        public async Task<List<Car>> GetByCustomerIdAsync(Guid customerId)
        {
            return await _context.Cars
                .Where(c => c.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<Car?> GetByIdAsync(
            Guid customerId,
            Guid carId)
        {
            return await _context.Cars
                .FirstOrDefaultAsync(c =>
                    c.CarId == carId &&
                    c.CustomerId == customerId);
        }

        public async Task AddAsync(Car car)
        {
            await _context.Cars.AddAsync(car);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Car car)
        {
            _context.Cars.Update(car);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Car car)
        {
            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
        }
    }
}