namespace NotificationService.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(
            string toEmail,
            string customerName,
            string bookingId,
            string serviceType,
            decimal amount);
    }
}