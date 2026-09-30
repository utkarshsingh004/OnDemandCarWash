using AuthService.Clients;
using AuthService.DTOs;
using AuthService.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Services
{
    public class AuthService : IAuthService
    {
        private readonly AdminDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly CustomerApiClient _customerApiClient;
        private readonly WasherApiClient _washerApiClient;

        public AuthService(
     AdminDbContext context,
     IConfiguration configuration,
     CustomerApiClient customerApiClient,
     WasherApiClient washerApiClient)
        {
            _context = context;
            _configuration = configuration;
            _customerApiClient = customerApiClient;
            _washerApiClient = washerApiClient;
        }

        public async Task<string> AdminLoginAsync(LoginDto dto)
        {
            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.Email == dto.Email);

            if (admin == null)
                throw new Exception("Invalid email or password");

            var passwordHasher = new PasswordHasher<Models.Admin>();

            var result =
                passwordHasher.VerifyHashedPassword(
                    admin,
                    admin.PasswordHash,
                    dto.Password);

            if (result == PasswordVerificationResult.Failed)
                throw new Exception("Invalid email or password");

            var claims = new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    admin.AdminId.ToString()),

                new Claim(
                    ClaimTypes.Name,
                    admin.Name),

                new Claim(
                    ClaimTypes.Email,
                    admin.Email),

                new Claim(
                    ClaimTypes.Role,
                    admin.Role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]!));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    double.Parse(
                        _configuration["Jwt:ExpiryMinutes"]!)),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }

        public async Task<CustomerResponseDto?> RegisterCustomerAsync(
    RegisterCustomerDto dto)
        {
            return await _customerApiClient.RegisterAsync(dto);
        }

        public async Task<string> CustomerLoginAsync(
    LoginCustomerDto dto)
        {
            var customer =
                await _customerApiClient.LoginAsync(dto);

            if (customer == null)
                throw new Exception("Invalid email or password");

            var claims = new[]
            {
        new Claim(
            ClaimTypes.NameIdentifier,
            customer.CustomerId.ToString()),

        new Claim(
            ClaimTypes.Name,
            customer.Name),

        new Claim(
            ClaimTypes.Email,
            customer.Email),

        new Claim(
            ClaimTypes.Role,
            "Customer")
    };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]!));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    double.Parse(
                        _configuration["Jwt:ExpiryMinutes"]!)),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }

        public async Task<WasherResponseDto?> RegisterWasherAsync(
    RegisterWasherDto dto)
        {
            return await _washerApiClient.RegisterAsync(dto);
        }

        public async Task<string> WasherLoginAsync(
    LoginWasherDto dto)
        {
            var washer =
                await _washerApiClient.LoginAsync(dto);

            if (washer == null)
                throw new Exception("Invalid email or password");

            var claims = new[]
            {
        new Claim(
            ClaimTypes.NameIdentifier,
            washer.WasherId.ToString()),

        new Claim(
            ClaimTypes.Name,
            washer.Name),

        new Claim(
            ClaimTypes.Email,
            washer.Email),

        new Claim(
            ClaimTypes.Role,
            "Washer")
    };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]!));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    double.Parse(
                        _configuration["Jwt:ExpiryMinutes"]!)),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }


    }
}