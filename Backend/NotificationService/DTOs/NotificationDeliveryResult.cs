using NotificationService.Models;

namespace NotificationService.DTOs
{
    public class NotificationDeliveryResult
    {
        public Notification Notification { get; set; } = new();

        public bool EmailSent { get; set; }

        public string? EmailError { get; set; }
    }
}
