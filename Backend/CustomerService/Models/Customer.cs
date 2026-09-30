namespace CustomerService.Models
{
    public class Customer
    {
        public Guid CustomerId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        // One Customer can have many Cars
        public ICollection<Car> Cars { get; set; } = new List<Car>();
    }
}