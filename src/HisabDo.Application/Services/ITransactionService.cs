using HisabDo.Application.DTOs;

namespace HisabDo.Application.Services;

public interface ITransactionService
{
    Task<IEnumerable<TransactionDto>> GetAllAsync(int userId);
    Task<TransactionDto?> GetByIdAsync(int id);
    Task<TransactionDto> CreateAsync(int userId, CreateTransactionDto dto);
    Task<TransactionDto> UpdateAsync(int id, CreateTransactionDto dto);
    Task DeleteAsync(int id);
}