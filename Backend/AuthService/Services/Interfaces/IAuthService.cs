using AuthService.DTOs;

namespace AuthService.Services
{
    public interface IAuthService
    {
        Task<string> AdminLoginAsync(LoginDto dto);

        Task<CustomerResponseDto?> RegisterCustomerAsync(
            RegisterCustomerDto dto);

        Task<string> CustomerLoginAsync(
            LoginCustomerDto dto);

        Task<WasherResponseDto?> RegisterWasherAsync(
            RegisterWasherDto dto);

        Task<string> WasherLoginAsync(
            LoginWasherDto dto);
    }
}