using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using BookingService.DTOs;

namespace BookingService.Clients
{
    public class CustomerApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomerApiClient(
            HttpClient httpClient,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<CarDto>?> GetMyCarsAsync()
        {
            var token =
                _httpContextAccessor.HttpContext?
                .Request.Headers["Authorization"]
                .ToString();

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    AuthenticationHeaderValue.Parse(token);
            }

            return await _httpClient.GetFromJsonAsync<List<CarDto>>(
                "api/Customer/cars");
        }
    }
}