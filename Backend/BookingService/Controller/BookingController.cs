using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BookingService.DTOs;
using BookingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookingService.Clients;

namespace BookingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly CustomerApiClient _customerApiClient;

        public BookingController(
    IBookingService bookingService,
    CustomerApiClient customerApiClient)
        {
            _bookingService = bookingService;
            _customerApiClient = customerApiClient;
        }

        private Guid GetCustomerIdFromToken()
        {
            var customerId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            return Guid.Parse(customerId!);
        }

        private Guid GetWasherIdFromToken()
        {
            var washerId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            return Guid.Parse(washerId!);
        }

        // GET: api/Booking
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var bookings =
                await _bookingService.GetAllAsync();

            return Ok(bookings);
        }

        // GET: api/Booking/my-bookings
        [HttpGet("my-bookings")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMyBookings()
        {
            var customerId =
                GetCustomerIdFromToken();

            var bookings =
                await _bookingService
                    .GetByCustomerIdAsync(customerId);

            return Ok(bookings);
        }

        [HttpGet("my-cars")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMyCars()
        {
            var cars = await _customerApiClient.GetMyCarsAsync();

            return Ok(cars);
        }

        // GET: api/Booking/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var booking =
                await _bookingService.GetByIdAsync(id);

            if (booking == null)
                return NotFound("Booking not found");

            return Ok(booking);
        }

        // POST: api/Booking
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create(
             CreateBookingDto dto)
        {
            try
            {
                var customerId =
                    GetCustomerIdFromToken();

                var booking =
                    await _bookingService.CreateAsync(
                        customerId,
                        dto);

                return Ok(booking);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Booking/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Update(
             Guid id,
             CreateBookingDto dto)
        {
            try
            {
                var customerId =
                    GetCustomerIdFromToken();

                await _bookingService.UpdateAsync(
                    id,
                    customerId,
                    dto);

                return Ok(
                    "Booking updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("pending")]
        [Authorize(Roles = "Washer")]
        public async Task<IActionResult> GetPendingBookings()
        {
            var bookings = await _bookingService.GetPendingBookingsAsync();

            return Ok(bookings);
        }

        [HttpPut("{id}/respond")]
        [Authorize(Roles = "Washer")]
        public async Task<IActionResult> RespondToBooking(
    Guid id,
    BookingResponseDto dto)
        {
            try
            {
                var washerId = GetWasherIdFromToken();

                await _bookingService.RespondToBookingAsync(
                    id,
                    washerId,
                    dto.Accept
                );

                if (dto.Accept)
                    return Ok("Booking accepted successfully");

                return Ok("Booking rejected successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/Booking/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var customerId =
                    GetCustomerIdFromToken();

                await _bookingService.DeleteAsync(
                    id,
                    customerId);

                return Ok(
                    "Booking deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}