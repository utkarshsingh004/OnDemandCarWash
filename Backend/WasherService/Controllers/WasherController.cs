using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WasherService.DTOs;
using WasherService.Services;

namespace WasherService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WasherController : ControllerBase
    {
        private readonly IWasherService _washerService;
        private readonly IConfiguration _configuration;

        public WasherController(
            IWasherService washerService,
            IConfiguration configuration)
        {
            _washerService = washerService;
            _configuration = configuration;
        }

        private Guid GetWasherIdFromToken()
        {
            var washerId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(washerId))
            {
                throw new UnauthorizedAccessException(
                    "Invalid token.");
            }

            return Guid.Parse(washerId);
        }

        // GET: api/Washer/all
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var washers = await _washerService.GetAllAsync();

            return Ok(washers);
        }

        // GET: api/Washer/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var washer = await _washerService.GetByIdAsync(id);

            return Ok(washer);
        }

        // POST: api/Washer/register
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(
            RegisterWasherDto dto)
        {
            var washer =
                await _washerService.RegisterAsync(dto);

            return Ok(new
            {
                message = "Washer registered successfully",
                washer.WasherId,
                washer.Name,
                washer.Email
            });
        }

        // POST: api/Washer/login
        [AllowAnonymous]
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            LoginWasherDto dto)
        {
            var washer =
                await _washerService.LoginAsync(dto);

            return Ok(new
            {
                washer.WasherId,
                washer.Name,
                washer.Email,
                washer.Phone,
                washer.Address
            });
        }

        // PUT: api/Washer/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            Guid id,
            UpdateWasherDto dto)
        {
            await _washerService.UpdateAsync(
                id,
                dto);

            return Ok(new
            {
                message = "Washer updated successfully"
            });
        }

        // PUT: api/Washer/availability
        [Authorize(Roles = "Washer")]
        [HttpPut("availability")]
        public async Task<IActionResult> UpdateAvailability(
            bool isAvailable)
        {
            var washerId = GetWasherIdFromToken();

            await _washerService.UpdateAvailabilityAsync(
                washerId,
                isAvailable);

            return Ok(new
            {
                message =
                    "Availability updated successfully"
            });
        }

        // GET: api/Washer/available
        [AllowAnonymous]
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailable()
        {
            var washers =
                await _washerService.GetAvailableAsync();

            return Ok(washers);
        }

        // DELETE: api/Washer/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _washerService.DeleteAsync(id);

            return Ok(new
            {
                message = "Washer deleted successfully"
            });
        }
    }
}