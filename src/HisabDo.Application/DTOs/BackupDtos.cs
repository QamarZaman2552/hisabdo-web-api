using System.ComponentModel.DataAnnotations;
using HisabDo.Domain.Constants;

namespace HisabDo.Application.DTOs;

public class BackupFileDto
{
    public DateTime ExportedAt { get; set; }
    public string AppVersion { get; set; } = "1.0";
    public BackupSettingsDto? Settings { get; set; }
    public List<BackupCategoryDto> Categories { get; set; } = [];
    public List<BackupCustomerDto> Customers { get; set; } = [];
    public List<BackupTransactionDto> Transactions { get; set; } = [];
}

public class BackupSettingsDto
{
    public string CurrencyCode { get; set; } = Defaults.CurrencyCode;
    public string LanguageCode { get; set; } = Defaults.LanguageCode;
}

public class BackupCategoryDto
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public int OriginalId { get; set; }
}

public class BackupCustomerDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
    [StringLength(20)]
    public string Phone { get; set; } = string.Empty;
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;
    [StringLength(500)]
    public string Notes { get; set; } = string.Empty;
    public int OriginalId { get; set; }
}

public class BackupTransactionDto
{
    [Range(0.01, 999999999.99)]
    public decimal Amount { get; set; }
    [Required]
    [Range(1, 2)]
    public int Type { get; set; }
    [StringLength(500)]
    public string Note { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public int OriginalCategoryId { get; set; }
    public int? OriginalCustomerId { get; set; }
}

public class RestoreResultDto
{
    public int CategoriesImported { get; set; }
    public int CategoriesSkipped { get; set; }
    public int CustomersImported { get; set; }
    public int CustomersSkipped { get; set; }
    public int TransactionsImported { get; set; }
    public int TransactionsSkipped { get; set; }
}
