using AuthService.DTOs; 

namespace AuthService.Clients
{
    public class WasherApiClient
    {
        private readonly HttpClient _httpClient;

        public WasherApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<WasherResponseDto?> RegisterAsync(
            RegisterWasherDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/Washer/register",
                dto);

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<WasherResponseDto>();
        }

        public async Task<WasherResponseDto?> LoginAsync(
            LoginWasherDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/Washer/login",
                dto);

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<WasherResponseDto>();
        }
    }
}