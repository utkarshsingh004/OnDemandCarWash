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
            return await _context.Bookings
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
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
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetByWasherIdAsync(Guid washerId)
        {
            return await _context.Bookings
                .Where(b => b.WasherId == washerId)
                .OrderByDescending(b => b.BookingDate)
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
                .OrderBy(b => b.BookingDate)
                .ToListAsync();
        }

        public async Task CancelAsync(Guid bookingId, Guid customerId)
        {
            var affected = await _context.Bookings
                .Where(b => b.BookingId == bookingId &&
                            b.CustomerId == customerId &&
                            (b.Status == "Pending" || b.Status == "Accepted"))
                .ExecuteUpdateAsync(updates => updates
                    .SetProperty(b => b.Status, "Cancelled")
                    .SetProperty(b => b.UpdatedAt, DateTime.UtcNow));

            if (affected > 0)
                return;

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);
            if (booking == null)
                throw new KeyNotFoundException("Booking not found");
            if (booking.CustomerId != customerId)
                throw new UnauthorizedAccessException(
                    "You cannot cancel this booking");

            throw new InvalidOperationException(
                "Only pending or accepted bookings can be cancelled");
        }

        public async Task RespondToBookingAsync(
            Guid bookingId,
            Guid washerId,
            bool accept)
        {
            var updatedAt = DateTime.UtcNow;
            var affected = await _context.Bookings
                .Where(b => b.BookingId == bookingId && b.Status == "Pending")
                .ExecuteUpdateAsync(updates => updates
                    .SetProperty(b => b.WasherId, accept ? washerId : (Guid?)null)
                    .SetProperty(b => b.Status, accept ? "Accepted" : "Declined")
                    .SetProperty(b => b.UpdatedAt, updatedAt));

            if (affected == 0)
            {
                var exists = await _context.Bookings
                    .AnyAsync(b => b.BookingId == bookingId);
                if (!exists)
                    throw new KeyNotFoundException("Booking not found");

                throw new InvalidOperationException("Booking is no longer pending");
            }
        }

        public async Task UpdateStatusAsync(
            Guid bookingId,
            Guid washerId,
            string expectedStatus,
            string status)
        {
            var affected = await _context.Bookings
                .Where(b => b.BookingId == bookingId &&
                            b.WasherId == washerId &&
                            b.Status == expectedStatus)
                .ExecuteUpdateAsync(updates => updates
                    .SetProperty(b => b.Status, status)
                    .SetProperty(b => b.UpdatedAt, DateTime.UtcNow));

            if (affected == 0)
            {
                var exists = await _context.Bookings
                    .AnyAsync(b => b.BookingId == bookingId);
                if (!exists)
                    throw new KeyNotFoundException("Booking not found");

                throw new InvalidOperationException(
                    "Booking is not assigned to you or its status has changed");
            }
        }

    }
}