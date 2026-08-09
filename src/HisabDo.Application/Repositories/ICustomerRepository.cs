using HisabDo.Domain.Entities;

namespace HisabDo.Application.Repositories;

public interface ICustomerRepository
{
    Task<List<Customer>> GetAllAsync(int userId);
    Task<Customer?> GetByIdAsync(int id);
    Task<Customer> AddAsync(Customer customer);
    Task<Customer> UpdateAsync(Customer customer);
    Task RemoveAsync(Customer customer);
}