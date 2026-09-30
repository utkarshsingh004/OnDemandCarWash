using BookingService.Models;

namespace BookingService.Repositories
{
    public interface IBookingRepository
    {
        Task<List<Booking>> GetAllAsync();
        Task<Booking?> GetByIdAsync(Guid id);
        Task<List<Booking>> GetByCustomerIdAsync(Guid customerId);
        Task<List<Booking>> GetPendingBookingsAsync();

        Task AddAsync(Booking booking);
        Task UpdateAsync(Booking booking);
        Task DeleteAsync(Booking booking);

        Task RespondToBookingAsync(
            Guid bookingId,
            Guid washerId,
            bool accept);
    }
}