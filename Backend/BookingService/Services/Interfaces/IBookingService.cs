using BookingService.DTOs;
using BookingService.Models;

namespace BookingService.Services
{
    public interface IBookingService
    {
        Task<List<Booking>> GetAllAsync();
        Task<Booking?> GetByIdAsync(Guid id);
        Task<List<Booking>> GetByCustomerIdAsync(Guid customerId);
        Task<List<Booking>> GetPendingBookingsAsync();

        Task<Booking> CreateAsync(
            Guid customerId,
            CreateBookingDto dto);

        Task UpdateAsync(
            Guid id,
            Guid customerId,
            CreateBookingDto dto);

        Task DeleteAsync(
            Guid id,
            Guid customerId);

        Task RespondToBookingAsync(
            Guid bookingId,
            Guid washerId,
            bool accept);
    }
}