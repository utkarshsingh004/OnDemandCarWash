using Microsoft.AspNetCore.Mvc;
using NotificationService.DTOs;
using NotificationService.Services;

namespace NotificationService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(
            INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            CreateNotificationDto dto)
        {
            var notification =
                await _notificationService.CreateAsync(dto);

            return Ok(notification);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetByCustomerId(
            Guid customerId)
        {
            var notifications =
                await _notificationService
                    .GetByCustomerIdAsync(customerId);

            return Ok(notifications);
        }
    }
}