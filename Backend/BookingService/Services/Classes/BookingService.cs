using BookingService.DTOs;
using BookingService.Models;
using BookingService.Repositories;
using BookingService.Clients;
using Microsoft.Extensions.Logging;

namespace BookingService.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _repository;
        private readonly WasherApiClient _washerApiClient;
        private readonly CustomerApiClient _customerApiClient;
        private readonly NotificationApiClient _notificationApiClient;
        private readonly ILogger<BookingService> _logger;

        public BookingService(
            IBookingRepository repository,
            WasherApiClient washerApiClient,
            CustomerApiClient customerApiClient,
            NotificationApiClient notificationApiClient,
            ILogger<BookingService> logger)
        {
            _repository = repository;
            _washerApiClient = washerApiClient;
            _customerApiClient = customerApiClient;
            _notificationApiClient = notificationApiClient;
            _logger = logger;
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

        public async Task<List<Booking>> GetByWasherIdAsync(Guid washerId)
        {
            return await _repository.GetByWasherIdAsync(washerId);
        }

        public async Task<List<Booking>> GetPendingBookingsAsync()
        {
            return await _repository.GetPendingBookingsAsync();
        }

        public async Task<Booking> CreateAsync(
            Guid customerId,
            CreateBookingDto dto)
        {
            var price = await ValidateBookingAsync(dto);

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
                throw new KeyNotFoundException("Booking not found");
            }

            if (booking.CustomerId != customerId)
            {
                throw new UnauthorizedAccessException(
                    "You cannot update this booking");
            }

            if (booking.Status != "Pending")
                throw new InvalidOperationException(
                    "Only pending bookings can be changed");

            var price = await ValidateBookingAsync(dto);

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

        public async Task CancelAsync(
            Guid id,
            Guid customerId)
        {
            await _repository.CancelAsync(id, customerId);
        }

        public async Task RespondToBookingAsync(
            Guid bookingId,
            Guid washerId,
            bool accept)
        {
            await _repository.RespondToBookingAsync(
                bookingId,
                washerId,
                accept);

            if (accept)
            {
                var booking = await _repository.GetByIdAsync(bookingId);
                if (booking == null)
                    throw new KeyNotFoundException("Booking not found");

                await SendAcceptanceNotificationAsync(booking);
            }
        }

        public async Task<NotificationDeliveryResponse> ResendAcceptedNotificationAsync(
            Guid bookingId,
            Guid customerId)
        {
            var booking = await _repository.GetByIdAsync(bookingId);
            if (booking == null)
                throw new KeyNotFoundException("Booking not found");
            if (booking.CustomerId != customerId)
                throw new UnauthorizedAccessException(
                    "You cannot resend a notification for this booking");
            if (booking.Status != "Accepted")
                throw new InvalidOperationException(
                    "Only accepted bookings can have their confirmation resent");

            return await SendAcceptanceNotificationAsync(booking);
        }

        public async Task UpdateStatusAsync(
            Guid bookingId,
            Guid washerId,
            string status)
        {
            var booking = await _repository.GetByIdAsync(bookingId);
            if (booking == null)
                throw new KeyNotFoundException("Booking not found");

            var expectedStatus = status switch
            {
                "InProgress" => "Accepted",
                "Completed" => "InProgress",
                _ => throw new ArgumentException("Unsupported booking status")
            };

            if (booking.WasherId != washerId)
                throw new UnauthorizedAccessException(
                    "This booking is not assigned to you");

            await _repository.UpdateStatusAsync(
                bookingId,
                washerId,
                expectedStatus,
                status);
        }

        private async Task<decimal> ValidateBookingAsync(CreateBookingDto dto)
        {
            if (dto.CarId == Guid.Empty)
                throw new ArgumentException("A valid vehicle is required");

            if (dto.BookingDate <= DateTime.UtcNow)
                throw new ArgumentException("Booking time must be in the future");

            if (dto.ServiceType is not (ServiceType.Basic or ServiceType.Premium))
                throw new ArgumentException("Invalid service type");

            var cars = await _customerApiClient.GetMyCarsAsync();
            if (cars == null || !cars.Any(car => car.CarId == dto.CarId))
                throw new UnauthorizedAccessException(
                    "The selected vehicle does not belong to your account");

            if (!dto.IsScheduled)
            {
                var washers = await _washerApiClient.GetAvailableWashersAsync();
                if (washers.Count == 0)
                    throw new InvalidOperationException(
                        "No washer is currently available for an on-demand wash");
            }

            return dto.ServiceType == ServiceType.Basic ? 499 : 799;
        }

        private async Task<NotificationDeliveryResponse> SendAcceptanceNotificationAsync(
            Booking booking)
        {
            try
            {
                var delivery =
                    await _notificationApiClient.SendBookingAcceptedNotificationAsync(
                        booking.CustomerId,
                        booking.BookingId,
                        booking.ServiceType,
                        booking.Price);

                if (!delivery.EmailSent)
                {
                    _logger.LogWarning(
                        "Booking {BookingId} was accepted, but its notification email was not sent: {EmailError}",
                        booking.BookingId,
                        delivery.EmailError ?? "No delivery failure reason was returned");
                }

                return delivery;
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(
                    exception,
                    "Booking {BookingId} was accepted, but its notification could not be delivered",
                    booking.BookingId);
                return new NotificationDeliveryResponse
                {
                    EmailSent = false,
                    EmailError = "Notification service could not be reached; check BookingService logs."
                };
            }
            catch (TaskCanceledException exception)
            {
                _logger.LogError(
                    exception,
                    "Booking {BookingId} was accepted, but notification delivery timed out",
                    booking.BookingId);
                return new NotificationDeliveryResponse
                {
                    EmailSent = false,
                    EmailError = "Notification service request timed out."
                };
            }
        }
    }
}