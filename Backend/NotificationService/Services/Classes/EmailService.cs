using System.Globalization;
using System.Text.Encodings.Web;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;
using NotificationService.Models;

namespace NotificationService.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendEmailAsync(
            string toEmail,
            string customerName,
            string bookingId,
            string serviceType,
            decimal amount)
        {
            ValidateSettings();

            var encodedCustomerName = HtmlEncoder.Default.Encode(customerName);
            var encodedBookingId = HtmlEncoder.Default.Encode(bookingId);
            var encodedServiceType = HtmlEncoder.Default.Encode(serviceType);
            var formattedAmount = amount.ToString("N2", CultureInfo.InvariantCulture);
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress(
                _settings.SenderName,
                _settings.SenderEmail));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = $"Green Car Wash - Booking {bookingId} accepted";
            email.Body = new TextPart("html")
            {
                Text = $$"""
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <meta charset="UTF-8">
                        <style>
                            body { font-family: Arial, sans-serif; background: #f4f6f8; padding: 20px; }
                            .container { max-width: 600px; margin: auto; background: #fff; border-radius: 10px; overflow: hidden; }
                            .header { background: #198754; color: #fff; text-align: center; padding: 24px; }
                            .content { padding: 28px; }
                            .details { width: 100%; border-collapse: collapse; }
                            .details td { padding: 12px; border-bottom: 1px solid #eee; }
                            .details td:first-child { font-weight: bold; color: #555; }
                        </style>
                    </head>
                    <body>
                        <div class="container">
                            <div class="header"><h1>Green Car Wash</h1></div>
                            <div class="content">
                                <h2>Hello {{encodedCustomerName}},</h2>
                                <p>Your wash request has been accepted. We look forward to seeing you.</p>
                                <table class="details">
                                    <tr><td>Booking ID</td><td>{{encodedBookingId}}</td></tr>
                                    <tr><td>Service</td><td>{{encodedServiceType}}</td></tr>
                                    <tr><td>Amount</td><td>₹{{formattedAmount}}</td></tr>
                                </table>
                            </div>
                        </div>
                    </body>
                    </html>
                    """
            };

            using var smtp = new SmtpClient();
            smtp.Timeout = 15000;
            var secureSocketOptions = _settings.Port == 465
                ? MailKit.Security.SecureSocketOptions.SslOnConnect
                : MailKit.Security.SecureSocketOptions.StartTls;
            await smtp.ConnectAsync(
                _settings.SmtpServer,
                _settings.Port,
                secureSocketOptions);
            await smtp.AuthenticateAsync(
                _settings.Username,
                _settings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        private void ValidateSettings()
        {
            var missingSettings = new List<string>();

            if (string.IsNullOrWhiteSpace(_settings.SmtpServer))
                missingSettings.Add("EmailSettings:SmtpServer");
            if (_settings.Port <= 0)
                missingSettings.Add("EmailSettings:Port");
            if (string.IsNullOrWhiteSpace(_settings.SenderName))
                missingSettings.Add("EmailSettings:SenderName");
            if (string.IsNullOrWhiteSpace(_settings.SenderEmail))
                missingSettings.Add("EmailSettings:SenderEmail");
            if (string.IsNullOrWhiteSpace(_settings.Username))
                missingSettings.Add("EmailSettings:Username");
            if (string.IsNullOrWhiteSpace(_settings.Password))
                missingSettings.Add("EmailSettings:Password");

            if (missingSettings.Count > 0)
            {
                throw new InvalidOperationException(
                    $"Email configuration is incomplete. Set: {string.Join(", ", missingSettings)}.");
            }
        }
    }
}
