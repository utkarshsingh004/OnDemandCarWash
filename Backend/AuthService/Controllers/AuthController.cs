using AuthService.DTOs;
using AuthService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("admin/login")]
        [AllowAnonymous]
        public async Task<IActionResult> AdminLogin(
            LoginDto dto)
        {
            var token =
                     await _authService.AdminLoginAsync(dto);

            return Ok(new
            {
                message = "Admin login successful",
                token = token
            });
        }

        [HttpPost("customer/register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterCustomer(
    RegisterCustomerDto dto)
        {

            var customer =
                await _authService.RegisterCustomerAsync(dto);

            return Ok(new
            {
                message = "Customer registered successfully",
                customer
            });

        }

        [HttpPost("customer/login")]
        [AllowAnonymous]
        public async Task<IActionResult> CustomerLogin(LoginCustomerDto dto)
        {
            var token = await _authService.CustomerLoginAsync(dto);

            return Ok(new
            {
                message = "Customer login successful",
                token = token
            });
        }

        [HttpPost("washer/register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterWasher(
    RegisterWasherDto dto)
        {
            var washer =
                     await _authService.RegisterWasherAsync(dto);

            return Ok(new
            {
                message = "Washer registered successfully",
                washer
            });
        }

        [HttpPost("washer/login")]
        [AllowAnonymous]
        public async Task<IActionResult> WasherLogin(
            LoginWasherDto dto)
        {
            var token =
                    await _authService.WasherLoginAsync(dto);

            return Ok(new
            {
                message = "Washer login successful",
                token = token
            });
        }

    }
}