namespace NotificationService.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(
            string toEmail,
            string customerName,
            string bookingId,
            string carName,
            string registrationNumber,
            string serviceType,
            decimal amount);
    }
}