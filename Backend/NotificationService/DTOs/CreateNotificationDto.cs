using System.ComponentModel.DataAnnotations;

namespace NotificationService.DTOs
{
    public class CreateNotificationDto
    {
        [Required]
        public Guid CustomerId { get; set; }

        [Required]
        public Guid BookingId { get; set; }

        [Required]
        public string Message { get; set; } = string.Empty;

        [Required]
        public string Type { get; set; } = string.Empty;

        [Required]
        public string ServiceType { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }
    }
}