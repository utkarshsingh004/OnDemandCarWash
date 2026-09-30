namespace CustomerService.Models
{
    public class Car
    {
        public Guid CarId { get; set; }

        public Guid CustomerId { get; set; }

        public string Brand { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;

        public string RegistrationNumber { get; set; } = string.Empty;

        public string Color { get; set; } = string.Empty;

        public string CarType { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        // Each Car belongs to one Customer
        public Customer Customer { get; set; } = null!;
    }
}