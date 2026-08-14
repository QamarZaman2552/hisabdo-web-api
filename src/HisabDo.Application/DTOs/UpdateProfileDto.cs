using System.ComponentModel.DataAnnotations;

namespace HisabDo.Application.DTOs;

public class UpdateProfileDto
{
    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters.")]
    public string FullName { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Business name must not exceed 100 characters.")]
    public string? BusinessName { get; set; }

    [StringLength(20, ErrorMessage = "Phone must not exceed 20 characters.")]
    public string? Phone { get; set; }
}
