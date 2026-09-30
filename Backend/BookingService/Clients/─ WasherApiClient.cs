using System.Net.Http.Json;

namespace BookingService.Clients
{
    public class WasherApiClient
    {
        private readonly HttpClient _httpClient;

        public WasherApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<WasherResponse>> GetAvailableWashersAsync()
        {
            var response =
                await _httpClient.GetFromJsonAsync<List<WasherResponse>>(
                    "api/Washer/available"
                );

            return response ?? new List<WasherResponse>();
        }
    }

    public class WasherResponse
    {
        public Guid WasherId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public bool IsAvailable { get; set; }

        public int Priority { get; set; }
    }
}