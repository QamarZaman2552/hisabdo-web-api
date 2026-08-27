using HisabDo.Domain.Common;
using HisabDo.Domain.Constants;

namespace HisabDo.Domain.Entities;

public class Setting : BaseEntity
{
    public int UserId { get; set; }
    public string CurrencyCode { get; set; } = Defaults.CurrencyCode;
    public string LanguageCode { get; set; } = Defaults.LanguageCode;

    public User? User { get; set; }
}