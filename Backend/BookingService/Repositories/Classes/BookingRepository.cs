using BookingService.Data;
using BookingService.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly BookingDbContext _context;

        public BookingRepository(BookingDbContext context)
        {
            _context = context;
        }

        public async Task<List<Booking>> GetAllAsync()
        {
            return await _context.Bookings.ToListAsync();
        }

        public async Task<Booking?> GetByIdAsync(Guid id)
        {
            return await _context.Bookings
                .FirstOrDefaultAsync(b => b.BookingId == id);
        }

        public async Task<List<Booking>> GetByCustomerIdAsync(
            Guid customerId)
        {
            return await _context.Bookings
                .Where(b => b.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task AddAsync(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Booking>> GetPendingBookingsAsync()
        {
            return await _context.Bookings
                .Where(b => b.Status == "Pending")
                .ToListAsync();
        }

        public async Task RespondToBookingAsync(
    Guid bookingId,
    Guid washerId,
    bool accept)
        {
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null)
                throw new Exception("Booking not found");

            if (booking.Status != "Pending")
                throw new Exception("Booking is no longer pending");

            if (accept)
            {
                booking.WasherId = washerId;
                booking.Status = "Accepted";
            }
            else
            {
                // Reject hone par booking pending rahegi
                // taaki doosre washers accept kar saken
            }

            booking.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Booking booking)
        {
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
        }
    }
}