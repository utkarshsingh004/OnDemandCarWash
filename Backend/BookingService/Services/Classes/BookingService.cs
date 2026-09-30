using BookingService.DTOs;
using BookingService.Models;
using BookingService.Repositories;
using BookingService.Clients;

namespace BookingService.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _repository;
        private readonly WasherApiClient _washerApiClient;
        private readonly NotificationApiClient _notificationApiClient;

        public BookingService(
            IBookingRepository repository,
            WasherApiClient washerApiClient,
            NotificationApiClient notificationApiClient)
        {
            _repository = repository;
            _washerApiClient = washerApiClient;
            _notificationApiClient = notificationApiClient;
        }

        public async Task<List<Booking>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Booking?> GetByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<List<Booking>> GetByCustomerIdAsync(
            Guid customerId)
        {
            return await _repository.GetByCustomerIdAsync(customerId);
        }

        public async Task<List<Booking>> GetPendingBookingsAsync()
        {
            return await _repository.GetPendingBookingsAsync();
        }

        public async Task<Booking> CreateAsync(
            Guid customerId,
            CreateBookingDto dto)
        {
            decimal price;

            if (dto.ServiceType == ServiceType.Basic)
            {
                price = 499;
            }
            else if (dto.ServiceType == ServiceType.Premium)
            {
                price = 799;
            }
            else
            {
                throw new Exception("Invalid service type");
            }

            // Check available washers
            var washers =
                await _washerApiClient.GetAvailableWashersAsync();

            if (washers.Count == 0)
            {
                throw new Exception(
                    "No washer is currently available");
            }

            var booking = new Booking
            {
                BookingId = Guid.NewGuid(),

                CustomerId = customerId,

                CarId = dto.CarId,

                WasherId = null,

                ServiceId = Guid.Empty,

                ServiceType = dto.ServiceType.ToString(),

                BookingDate = dto.BookingDate,

                IsScheduled = dto.IsScheduled,

                Status = "Pending",

                Price = price,

                CreatedAt = DateTime.UtcNow,

                UpdatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(booking);

            return booking;
        }

        public async Task UpdateAsync(
            Guid id,
            Guid customerId,
            CreateBookingDto dto)
        {
            var booking =
                await _repository.GetByIdAsync(id);

            if (booking == null)
            {
                throw new Exception("Booking not found");
            }

            if (booking.CustomerId != customerId)
            {
                throw new Exception(
                    "You cannot update this booking");
            }

            decimal price;

            if (dto.ServiceType == ServiceType.Basic)
            {
                price = 499;
            }
            else if (dto.ServiceType == ServiceType.Premium)
            {
                price = 799;
            }
            else
            {
                throw new Exception("Invalid service type");
            }

            booking.CarId = dto.CarId;

            booking.ServiceType =
                dto.ServiceType.ToString();

            booking.BookingDate =
                dto.BookingDate;

            booking.IsScheduled =
                dto.IsScheduled;

            booking.Price = price;

            booking.UpdatedAt =
                DateTime.UtcNow;

            await _repository.UpdateAsync(booking);
        }

        public async Task DeleteAsync(
            Guid id,
            Guid customerId)
        {
            var booking =
                await _repository.GetByIdAsync(id);

            if (booking == null)
            {
                throw new Exception("Booking not found");
            }

            if (booking.CustomerId != customerId)
            {
                throw new Exception(
                    "You cannot delete this booking");
            }

            await _repository.DeleteAsync(booking);
        }

        public async Task RespondToBookingAsync(
            Guid bookingId,
            Guid washerId,
            bool accept)
        {
            // Accept or reject booking
            await _repository.RespondToBookingAsync(
                bookingId,
                washerId,
                accept);

            // If washer rejected, nothing else to do
            if (!accept)
            {
                return;
            }

            // Get updated booking
            var booking =
                await _repository.GetByIdAsync(bookingId);

            if (booking == null)
            {
                throw new Exception("Booking not found");
            }

            // Send notification to customer
            await _notificationApiClient
                .SendBookingAcceptedNotificationAsync(
                    booking.CustomerId,
                    booking.BookingId);

            // Demo washing process
            await Task.Delay(10000);

            // Mark washing as completed
            booking.Status = "Completed";

            booking.UpdatedAt =
                DateTime.UtcNow;

            await _repository.UpdateAsync(booking);
        }
    }
}