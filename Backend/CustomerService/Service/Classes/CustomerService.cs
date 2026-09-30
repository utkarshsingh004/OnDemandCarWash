using CustomerService.DTOs;
using CustomerService.Models;
using CustomerService.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace CustomerService.Services.Interfaces
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;
        private readonly PasswordHasher<Customer> _passwordHasher;

        public CustomerService(ICustomerRepository repository)
        {
            _repository = repository;
            _passwordHasher = new PasswordHasher<Customer>();
        }

        public async Task<List<Customer>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Customer?> GetByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Customer> RegisterAsync(RegisterCustomerDto dto)
        {
            var existingCustomer =
                await _repository.GetByEmailAsync(dto.Email);

            if (existingCustomer != null)
                throw new ArgumentException("Email already registered");

            var customer = new Customer
            {
                CustomerId = Guid.NewGuid(),
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            customer.PasswordHash =
                _passwordHasher.HashPassword(
                    customer,
                    dto.Password
                );

            await _repository.AddAsync(customer);

            return customer;
        }

        public async Task<Customer?> LoginAsync(LoginCustomerDto dto)
        {
            var customer =
                await _repository.GetByEmailAsync(dto.Email);

            if (customer == null)
                return null;

            var result =
                _passwordHasher.VerifyHashedPassword(
                    customer,
                    customer.PasswordHash,
                    dto.Password
                );

            if (result == PasswordVerificationResult.Failed)
                return null;

            return customer;
        }

        public async Task UpdateAsync(
            Guid id,
            UpdateCustomerDto dto)
        {
            var customer =
                await _repository.GetByIdAsync(id);

            if (customer == null)
                throw new Exception("Customer not found");

            customer.Name = dto.Name;
            customer.Email = dto.Email;
            customer.Phone = dto.Phone;
            customer.Address = dto.Address;
            customer.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(customer);
        }

        public async Task DeleteAsync(Guid id)
        {
            var customer =
                await _repository.GetByIdAsync(id);

            if (customer == null)
                throw new Exception("Customer not found");

            await _repository.DeleteAsync(customer);
        }
    }
}