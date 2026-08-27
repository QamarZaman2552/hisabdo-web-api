using System.ComponentModel.DataAnnotations;

namespace HisabDo.Application.DTOs;

public class UpdateUserDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string FullName { get; set; } = string.Empty;

    [StringLength(20)]
    public string? Role { get; set; }
}
