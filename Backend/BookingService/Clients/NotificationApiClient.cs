using System.Net.Http.Json;

namespace BookingService.Clients
{
    public class NotificationApiClient
    {
        private readonly HttpClient _httpClient;

        public NotificationApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task SendBookingAcceptedNotificationAsync(
            Guid customerId,
            Guid bookingId)
        {
            var notification = new
            {
                customerId = customerId,
                bookingId = bookingId,
                message = "Your booking has been accepted.",
                type = "BookingAccepted"
            };

            await _httpClient.PostAsJsonAsync(
                "api/Notification",
                notification);
        }
    }
}