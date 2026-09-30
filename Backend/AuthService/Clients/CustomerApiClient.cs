using AuthService.DTOs;

namespace AuthService.Clients
{
    public class CustomerApiClient
    {
        private readonly HttpClient _httpClient;

        public CustomerApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CustomerResponseDto?> RegisterAsync(
            RegisterCustomerDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/Customer/register",
                dto);

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<CustomerResponseDto>();
        }

        public async Task<CustomerResponseDto?> LoginAsync(
    LoginCustomerDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/Customer/login",
                dto);

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<CustomerResponseDto>();
        }
    }
}