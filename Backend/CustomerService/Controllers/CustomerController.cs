using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CustomerService.DTOs;
using CustomerService.Services.Interfaces;

namespace CustomerService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ICarService _carService;
        private readonly IConfiguration _configuration;

        public CustomerController(
            ICustomerService customerService,
            ICarService carService,
            IConfiguration configuration)
        {
            _customerService = customerService;
            _carService = carService;
            _configuration = configuration;
        }

        private Guid GetCustomerIdFromToken()
        {
            var customerId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(customerId))
                throw new UnauthorizedAccessException("Invalid token");

            return Guid.Parse(customerId);
        }

        // GET: api/Customer/all
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await _customerService.GetAllAsync();

            return Ok(customers);
        }

        // GET: api/Customer/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCustomer(Guid id)
        {
            var customer = await _customerService.GetByIdAsync(id);

            return Ok(customer);
        }

        // POST: api/Customer/register
        [AllowAnonymous]
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterCustomerDto dto)
        {
            var customer = await _customerService.RegisterAsync(dto);

            return Ok(new
            {
                message = "Customer registered successfully",
                customerId = customer.CustomerId,
                name = customer.Name,
                email = customer.Email
            });
        }

        // POST: api/Customer/login
        [AllowAnonymous]
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginCustomerDto dto)
        {
            var customer = await _customerService.LoginAsync(dto);

            return Ok(new
            {
                customer.CustomerId,
                customer.Name,
                customer.Email,
                customer.Phone,
                customer.Address
            });
        }

        // PUT: api/Customer/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(
            Guid id,
            UpdateCustomerDto dto)
        {
            await _customerService.UpdateAsync(id, dto);

            return Ok(new
            {
                message = "Customer updated successfully"
            });
        }

        // DELETE: api/Customer/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            await _customerService.DeleteAsync(id);

            return Ok(new
            {
                message = "Customer deleted successfully"
            });
        }

        // POST: api/Customer/cars
        [HttpPost("cars")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddCar(CreateCarDto dto)
        {
            var customerId = GetCustomerIdFromToken();

            var car = await _carService.AddCarAsync(
                customerId,
                dto);

            return Ok(car);
        }

        // GET: api/Customer/cars
        [HttpGet("cars")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetCustomerCars()
        {
            var customerId = GetCustomerIdFromToken();

            var cars = await _carService.GetCustomerCarsAsync(
                customerId);

            return Ok(cars);
        }

        // PUT: api/Customer/cars/{carId}
        [HttpPut("cars/{carId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> UpdateCar(
            Guid carId,
            UpdateCarDto dto)
        {
            var customerId = GetCustomerIdFromToken();

            await _carService.UpdateCarAsync(
                customerId,
                carId,
                dto);

            return Ok(new
            {
                message = "Car updated successfully"
            });
        }

        // DELETE: api/Customer/cars/{carId}
        [HttpDelete("cars/{carId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> DeleteCar(Guid carId)
        {
            var customerId = GetCustomerIdFromToken();

            await _carService.DeleteCarAsync(
                customerId,
                carId);

            return Ok(new
            {
                message = "Car deleted successfully"
            });
        }
    }
}