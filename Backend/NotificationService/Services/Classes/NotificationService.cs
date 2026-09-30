using NotificationService.DTOs;
using NotificationService.Models;


namespace NotificationService.Services
{
    public class NotificationService : INotificationService
    {
        private readonly List<Notification> _notifications = new();
        private readonly IEmailService _emailService;

        public NotificationService(IEmailService emailService)
        {
            _emailService = emailService;
        }
        public async Task<Notification> CreateAsync(
            CreateNotificationDto dto)
        {
            var notification = new Notification
            {
                NotificationId = Guid.NewGuid(),
                CustomerId = dto.CustomerId,
                BookingId = dto.BookingId,
                Message = dto.Message,
                Type = dto.Type,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            _notifications.Add(notification);

            await _emailService.SendEmailAsync(
    dto.Email,
    "Customer",
    dto.BookingId.ToString(),
    "Car",
    "N/A",
    dto.Type,
    0);

            return await Task.FromResult(notification);
        }

        public async Task<List<Notification>> GetByCustomerIdAsync(
            Guid customerId)
        {
            var notifications = _notifications
                .Where(n => n.CustomerId == customerId)
                .ToList();

            return await Task.FromResult(notifications);
        }
    }
}