using System.ComponentModel.DataAnnotations;

namespace HisabDo.Application.DTOs;

public class CreateCategoryDto
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 50 characters.")]
    public string Name { get; set; } = string.Empty;
}
