namespace CustomerService.DTOs
{
    public class UpdateCarDto
    {
        public string Brand { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;

        public string RegistrationNumber { get; set; } = string.Empty;

        public string Color { get; set; } = string.Empty;

        public string CarType { get; set; } = string.Empty;
    }
}