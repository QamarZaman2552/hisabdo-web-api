namespace HisabDo.Application.DTOs;

public class NotificationSummaryDto
{
    public PeriodSummaryDto Today { get; set; } = new();
    public PeriodSummaryDto ThisWeek { get; set; } = new();
}

public class PeriodSummaryDto
{
    public decimal Receivable { get; set; }
    public decimal Payable { get; set; }
    public int Transactions { get; set; }
}