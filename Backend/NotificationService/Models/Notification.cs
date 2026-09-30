namespace NotificationService.Models
{
    public class Notification
    {
        public Guid NotificationId { get; set; }

        public Guid CustomerId { get; set; }

        public Guid BookingId { get; set; }

        public string Message { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public bool IsRead { get; set; }
    }
}