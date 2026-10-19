using System.Net.Http.Json;
using NotificationService.DTOs;

namespace NotificationService.Clients
{
    public class CustomerApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CustomerApiClient> _logger;

        public CustomerApiClient(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<CustomerApiClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<CustomerNotificationDetailsDto>
            GetNotificationDetailsAsync(Guid customerId)
        {
            var apiKey = _configuration["InternalApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException(
                    "The InternalApiKey setting must be configured for both NotificationService and CustomerService.");
            }

            var client = _httpClientFactory.CreateClient("CustomerService");
            using var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"api/Customer/internal/{customerId}/notification-details");
            request.Headers.Add("X-Internal-Api-Key", apiKey);

            using var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "Customer details lookup failed with status code {StatusCode}",
                    (int)response.StatusCode);
                throw new HttpRequestException(
                    "Customer details lookup failed.",
                    null,
                    response.StatusCode);
            }

            return await response.Content
                .ReadFromJsonAsync<CustomerNotificationDetailsDto>()
                ?? throw new InvalidOperationException(
                    "Customer details were not returned.");
        }
    }
}