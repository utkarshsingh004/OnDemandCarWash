namespace BookingService.Models
{
    public class Booking
    {
        public Guid BookingId { get; set; }

        // References to Customer Service
        public Guid CustomerId { get; set; }

        public Guid CarId { get; set; }

        // References to Washer Service
        public Guid? WasherId { get; set; }

        // References to Service Catalog Service
        public Guid ServiceId { get; set; }

        // Basic / Premium
        public string ServiceType { get; set; } = string.Empty;

        public DateTime BookingDate { get; set; }

        public bool IsScheduled { get; set; }

        public string Status { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}