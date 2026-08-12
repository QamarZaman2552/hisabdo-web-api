using System.ComponentModel.DataAnnotations;

namespace HisabDo.Application.DTOs;

public class UpdateSettingDto
{
    [Required(ErrorMessage = "Currency code is required.")]
    [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency code must be 3 characters (e.g. PKR).")]
    public string CurrencyCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Language code is required.")]
    [StringLength(2, MinimumLength = 2, ErrorMessage = "Language code must be 2 characters (e.g. en).")]
    public string LanguageCode { get; set; } = string.Empty;
}