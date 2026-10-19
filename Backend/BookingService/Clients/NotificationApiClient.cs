using System.Net.Http.Json;
using System.Text.Json;

namespace BookingService.Clients
{
    public sealed class NotificationDeliveryResponse
    {
        public bool EmailSent { get; set; }

        public string? EmailError { get; set; }

        public NotificationServiceRecord? Notification { get; set; }
    }

    public sealed class NotificationServiceRecord
    {
        public Guid NotificationId { get; set; }
    }

    public class NotificationApiClient
    {
        private readonly HttpClient _httpClient;

        public NotificationApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<NotificationDeliveryResponse> SendBookingAcceptedNotificationAsync(
            Guid customerId,
            Guid bookingId,
            string serviceType,
            decimal amount)
        {
            var notification = new
            {
                customerId = customerId,
                bookingId = bookingId,
                message = "Your booking has been accepted.",
                type = "BookingAccepted",
                serviceType = serviceType,
                amount = amount
            };

            using var response = await _httpClient.PostAsJsonAsync(
                "api/Notification",
                notification);

            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var responseDetail = string.IsNullOrWhiteSpace(responseBody)
                    ? response.ReasonPhrase
                    : responseBody;
                throw new HttpRequestException(
                    $"Notification service returned {(int)response.StatusCode}: {responseDetail}",
                    null,
                    response.StatusCode);
            }

            return await response.Content
                .ReadFromJsonAsync<NotificationDeliveryResponse>()
                ?? throw new JsonException(
                    "Notification service returned an empty delivery response.");
        }
    }
}