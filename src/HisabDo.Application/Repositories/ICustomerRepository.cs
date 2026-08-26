using HisabDo.Domain.Entities;

namespace HisabDo.Application.Repositories;

public interface ICustomerRepository
{
    Task<(List<Customer> Items, int TotalCount)> GetAllAsync(int userId, int page = 1, int pageSize = 50);
    Task<Customer?> GetByIdAsync(int userId, int id);
    Task<bool> HasTransactionsAsync(int customerId);
    Task<Customer> AddAsync(Customer customer);
    Task<Customer> UpdateAsync(Customer customer);
    Task RemoveAsync(Customer customer);
}