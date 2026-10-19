using System.ComponentModel.DataAnnotations;

namespace BookingService.DTOs
{
    public class UpdateBookingStatusDto
    {
        [Required]
        public string Status { get; set; } = string.Empty;
    }
}
