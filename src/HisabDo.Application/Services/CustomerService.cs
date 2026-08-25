using System.ComponentModel.DataAnnotations;
using HisabDo.Application.DTOs;
using HisabDo.Application.Repositories;
using HisabDo.Domain.Entities;

namespace HisabDo.Application.Services;

public class CustomerService(ICustomerRepository repository) : ICustomerService
{
    public async Task<PaginatedResult<CustomerDto>> GetAllAsync(int userId, int page = 1, int pageSize = 50)
    {
        var (customers, totalCount) = await repository.GetAllAsync(userId, page, pageSize);
        return new PaginatedResult<CustomerDto>
        {
            Items = customers.Select(ToDto).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<CustomerDto?> GetByIdAsync(int userId, int id)
    {
        var customer = await repository.GetByIdAsync(userId, id);
        return customer == null ? null : ToDto(customer);
    }

    public async Task<CustomerDto> CreateAsync(int userId, CreateCustomerDto dto)
    {
        EnsureEmailIsValid(dto.Email);

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

    public async Task<CustomerDto> UpdateAsync(int userId, int id, CreateCustomerDto dto)
    {
        EnsureEmailIsValid(dto.Email);

        var customer = await repository.GetByIdAsync(userId, id)
            ?? throw new KeyNotFoundException($"No customer found with ID: {id}");

        customer.Name = dto.Name;
        customer.Phone = dto.Phone;
        customer.Email = dto.Email;
        customer.Notes = dto.Notes;

        await repository.UpdateAsync(customer);
        return ToDto(customer);
    }

    public async Task DeleteAsync(int userId, int id)
    {
        var customer = await repository.GetByIdAsync(userId, id)
            ?? throw new KeyNotFoundException($"No customer found with ID: {id}");

        await repository.RemoveAsync(customer);
    }

    private static void EnsureEmailIsValid(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return;
        }

        if (!new EmailAddressAttribute().IsValid(email))
        {
            throw new InvalidOperationException("Email is not in a valid format.");
        }
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