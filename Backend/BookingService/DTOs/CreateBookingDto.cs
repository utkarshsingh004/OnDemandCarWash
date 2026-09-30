using BookingService.Models;

namespace BookingService.DTOs
{
    public class CreateBookingDto
    {
        public Guid CarId { get; set; }

        public ServiceType ServiceType { get; set; }

        public DateTime BookingDate { get; set; }

        public bool IsScheduled { get; set; }
    }
}