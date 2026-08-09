using System.ComponentModel.DataAnnotations;

namespace HisabDo.Application.DTOs;

public class CreateCustomerDto
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "Phone must not exceed 20 characters.")]
    public string Phone { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Email is not in a valid format.")]
    [StringLength(100, ErrorMessage = "Email must not exceed 100 characters.")]
    public string Email { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Notes must not exceed 500 characters.")]
    public string Notes { get; set; } = string.Empty;
}