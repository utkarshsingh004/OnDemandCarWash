using NotificationService.DTOs;
using NotificationService.Models;

namespace NotificationService.Services
{
    public interface INotificationService
    {
        Task<Notification> CreateAsync(
            CreateNotificationDto dto);

        Task<List<Notification>> GetByCustomerIdAsync(
            Guid customerId);
    }
}