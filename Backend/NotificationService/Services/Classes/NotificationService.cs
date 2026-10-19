using NotificationService.DTOs;
using NotificationService.Models;
using NotificationService.Clients;
using System.Collections.Concurrent;
using System.Net.Sockets;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace NotificationService.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ConcurrentBag<Notification> _notifications = new();
        private readonly IEmailService _emailService;
        private readonly CustomerApiClient _customerApiClient;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            IEmailService emailService,
            CustomerApiClient customerApiClient,
            ILogger<NotificationService> logger)
        {
            _emailService = emailService;
            _customerApiClient = customerApiClient;
            _logger = logger;
        }
        public async Task<NotificationDeliveryResult> CreateAsync(
            CreateNotificationDto dto)
        {
            var notification = new Notification
            {
                NotificationId = Guid.NewGuid(),
                CustomerId = dto.CustomerId,
                BookingId = dto.BookingId,
                Message = dto.Message,
                Type = dto.Type,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            _notifications.Add(notification);

            try
            {
                var customer = await _customerApiClient
                    .GetNotificationDetailsAsync(dto.CustomerId);

                await _emailService.SendEmailAsync(
                    customer.Email,
                    customer.Name,
                    dto.BookingId.ToString(),
                    dto.ServiceType,
                    dto.Amount);

                return new NotificationDeliveryResult
                {
                    Notification = notification,
                    EmailSent = true
                };
            }
            catch (Exception exception) when (
                exception is HttpRequestException or
                    InvalidOperationException or
                    FormatException or
                    IOException or
                    SocketException or
                    TimeoutException or
                    OperationCanceledException or
                    SmtpCommandException or
                    SmtpProtocolException or
                    AuthenticationException)
            {
                _logger.LogError(
                    exception,
                    "Could not send booking notification email for booking {BookingId} to customer {CustomerId}",
                    dto.BookingId,
                    dto.CustomerId);

                return new NotificationDeliveryResult
                {
                    Notification = notification,
                    EmailSent = false,
                    EmailError = GetDeliveryError(exception)
                };
            }
        }

        public async Task<List<Notification>> GetByCustomerIdAsync(
            Guid customerId)
        {
            var notifications = _notifications
                .Where(n => n.CustomerId == customerId)
                .ToList();

            return notifications;
        }

        private static string GetDeliveryError(Exception exception) =>
            exception switch
            {
                HttpRequestException =>
                    "Could not retrieve customer email; check CustomerService URL and InternalApiKey.",
                InvalidOperationException configException
                    when configException.Message.StartsWith(
                        "Email configuration is incomplete",
                        StringComparison.Ordinal) =>
                    configException.Message,
                InvalidOperationException =>
                    "Notification configuration is incomplete; verify InternalApiKey and CustomerService settings.",
                AuthenticationException =>
                    "SMTP authentication failed; verify the account and app password.",
                SmtpCommandException =>
                    "The SMTP server rejected the email; verify the sender and recipient addresses.",
                SmtpProtocolException or SocketException or IOException or TimeoutException =>
                    "Could not connect to or send through the SMTP server; verify host, port, and TLS settings.",
                FormatException =>
                    "The customer email address is invalid.",
                OperationCanceledException =>
                    "Email delivery timed out or was cancelled.",
                _ => "Email delivery failed; check NotificationService logs."
            };
    }
}