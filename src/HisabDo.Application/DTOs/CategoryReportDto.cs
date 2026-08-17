namespace HisabDo.Application.DTOs;

public class CategoryReportDto
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int TransactionCount { get; set; }
    public decimal ReceivableTotal { get; set; }
    public decimal PayableTotal { get; set; }
}