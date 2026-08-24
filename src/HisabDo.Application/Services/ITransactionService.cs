using HisabDo.Application.DTOs;

namespace HisabDo.Application.Services;

public interface ITransactionService
{
    Task<PaginatedResult<TransactionDto>> GetAllAsync(int userId, TransactionFilterDto filter);
    Task<IEnumerable<TransactionDto>> GetByCategoryAsync(int userId, int categoryId);
    Task<TransactionDto?> GetByIdAsync(int id);
    Task<TransactionDto> CreateAsync(int userId, CreateTransactionDto dto);
    Task<TransactionDto> UpdateAsync(int userId, int id, CreateTransactionDto dto);
    Task DeleteAsync(int userId, int id);
    Task UpdateAttachmentUrlAsync(int userId, int transactionId, string attachmentUrl);
    Task<string?> GetAttachmentUrlAsync(int userId, int transactionId);
}