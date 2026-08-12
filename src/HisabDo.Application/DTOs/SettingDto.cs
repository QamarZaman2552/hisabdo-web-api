using System.ComponentModel.DataAnnotations;

namespace HisabDo.Application.DTOs;

public class SettingDto
{
    public string CurrencyCode { get; set; } = string.Empty;
    public string LanguageCode { get; set; } = string.Empty;
}