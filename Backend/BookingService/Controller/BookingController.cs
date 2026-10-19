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
    [Authorize]
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

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var bookings =
                await _bookingService.GetAllAsync();

            return Ok(bookings);
        }

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

        [HttpGet("my-assignments")]
        [Authorize(Roles = "Washer")]
        public async Task<IActionResult> GetMyAssignments()
        {
            var washerId = GetWasherIdFromToken();
            var bookings = await _bookingService.GetByWasherIdAsync(washerId);
            return Ok(bookings);
        }

        [HttpGet("my-cars")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMyCars()
        {
            var cars = await _customerApiClient.GetMyCarsAsync();

            return Ok(cars);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var booking =
                await _bookingService.GetByIdAsync(id);

            if (booking == null)
                return NotFound("Booking not found");

            if (!User.IsInRole("Admin") &&
                !(User.IsInRole("Customer") &&
                  booking.CustomerId == GetCustomerIdFromToken()) &&
                !(User.IsInRole("Washer") &&
                  booking.WasherId == GetWasherIdFromToken()))
            {
                return NotFound("Booking not found");
            }

            return Ok(booking);
        }

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

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = booking.BookingId },
                    booking);
            }
            catch (Exception ex) when (IsExpectedBookingError(ex))
            {
                return MapBookingError(ex);
            }
        }

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
            catch (Exception ex) when (IsExpectedBookingError(ex))
            {
                return MapBookingError(ex);
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
            catch (Exception ex) when (IsExpectedBookingError(ex))
            {
                return MapBookingError(ex);
            }
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Washer")]
        public async Task<IActionResult> UpdateStatus(
            Guid id,
            UpdateBookingStatusDto dto)
        {
            try
            {
                await _bookingService.UpdateStatusAsync(
                    id,
                    GetWasherIdFromToken(),
                    dto.Status);

                return NoContent();
            }
            catch (Exception ex) when (IsExpectedBookingError(ex))
            {
                return MapBookingError(ex);
            }
        }

        [HttpPost("{id}/notification/retry")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> ResendAcceptedNotification(Guid id)
        {
            try
            {
                var delivery =
                    await _bookingService.ResendAcceptedNotificationAsync(
                        id,
                        GetCustomerIdFromToken());
                return Ok(delivery);
            }
            catch (Exception ex) when (IsExpectedBookingError(ex))
            {
                return MapBookingError(ex);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var customerId =
                    GetCustomerIdFromToken();

                await _bookingService.CancelAsync(
                    id,
                    customerId);

                return NoContent();
            }
            catch (Exception ex) when (IsExpectedBookingError(ex))
            {
                return MapBookingError(ex);
            }
        }

        private static bool IsExpectedBookingError(Exception exception) =>
            exception is KeyNotFoundException or
                UnauthorizedAccessException or
                InvalidOperationException or
                ArgumentException;

        private IActionResult MapBookingError(Exception exception) =>
            exception switch
            {
                KeyNotFoundException => NotFound(exception.Message),
                UnauthorizedAccessException =>
                    StatusCode(StatusCodes.Status403Forbidden, exception.Message),
                InvalidOperationException => Conflict(exception.Message),
                ArgumentException => BadRequest(exception.Message),
                _ => throw exception
            };
    }
}