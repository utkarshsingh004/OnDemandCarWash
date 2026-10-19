using NotificationService.DTOs;
using NotificationService.Models;

namespace NotificationService.Services
{
    public interface INotificationService
    {
        Task<NotificationDeliveryResult> CreateAsync(
            CreateNotificationDto dto);

        Task<List<Notification>> GetByCustomerIdAsync(
            Guid customerId);
    }
}