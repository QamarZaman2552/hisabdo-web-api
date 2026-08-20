using HisabDo.Application.DTOs;
using HisabDo.Application.Repositories;
using HisabDo.Domain.Entities;

namespace HisabDo.Application.Services;

public class CustomerService(ICustomerRepository repository) : ICustomerService
{
    public async Task<IEnumerable<CustomerDto>> GetAllAsync(int userId)
    {
        var customers = await repository.GetAllAsync(userId);
        return customers.Select(ToDto);
    }

    public async Task<CustomerDto?> GetByIdAsync(int id)
    {
        var customer = await repository.GetByIdAsync(id);
        return customer == null ? null : ToDto(customer);
    }

    public async Task<CustomerDto> CreateAsync(int userId, CreateCustomerDto dto)
    {
        var customer = new Customer
        {
            UserId = userId,
            Name = dto.Name,
            Phone = dto.Phone,
            Email = dto.Email,
            Notes = dto.Notes
        };

        await repository.AddAsync(customer);
        return ToDto(customer);
    }

    public async Task<CustomerDto> UpdateAsync(int id, CreateCustomerDto dto)
    {
        var customer = await repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"No customer found with ID: {id}");

        customer.Name = dto.Name;
        customer.Phone = dto.Phone;
        customer.Email = dto.Email;
        customer.Notes = dto.Notes;

        await repository.UpdateAsync(customer);
        return ToDto(customer);
    }

    public async Task DeleteAsync(int id)
    {
        var customer = await repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"No customer found with ID: {id}");

        await repository.RemoveAsync(customer);
    }

    private static CustomerDto ToDto(Customer customer)
    {
        return new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Phone = customer.Phone,
            Email = customer.Email,
            Notes = customer.Notes,
            CreatedAt = customer.CreatedAt
        };
    }
}