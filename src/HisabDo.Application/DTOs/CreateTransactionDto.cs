using System.ComponentModel.DataAnnotations;
using HisabDo.Domain.Enums;

namespace HisabDo.Application.DTOs;

public class CreateTransactionDto
{
    [Required(ErrorMessage = "CustomerId is required.")]
    public int CustomerId { get; set; }

    [Required(ErrorMessage = "CategoryId is required.")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Type is required.")]
    [Range(1, 2, ErrorMessage = "Type must be 1 (Receivable) or 2 (Payable).")]
    public TransactionType Type { get; set; }

    [Range(0.01, 999999999.99, ErrorMessage = "Amount must be greater than 0.")]
    public decimal Amount { get; set; }

    [StringLength(500, ErrorMessage = "Note must not exceed 500 characters.")]
    public string Note { get; set; } = string.Empty;

    public DateTime? TransactionDate { get; set; }
}