namespace HisabDo.Application.DTOs;

public class ReportSummaryDto
{
    public int TotalCustomers { get; set; }
    public int TotalCategories { get; set; }
    public int TotalTransactions { get; set; }
    public decimal TotalReceivable { get; set; }
    public decimal TotalPayable { get; set; }
    public decimal Balance { get; set; }
    public int ThisMonthTransactions { get; set; }
    public decimal ThisMonthReceived { get; set; }
    public decimal ThisMonthPaid { get; set; }
}