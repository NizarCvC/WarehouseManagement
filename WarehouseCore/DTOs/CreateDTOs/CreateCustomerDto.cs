using System.ComponentModel.DataAnnotations;

namespace WarehouseCore.DTOs.CreateDTOs;

public class CreateCustomerDto
{
    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public required string Name { get; set; }
    [Required(ErrorMessage = "Phone is required.")]
    [MaxLength(20, ErrorMessage = "Phone cannot exceed 20 characters.")]
    public required string Phone { get; set; }
    [Required(ErrorMessage = "Email is required.")]
    [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public required string Email { get; set; }
    [Required(ErrorMessage = "Address is required.")]
    [MaxLength(500, ErrorMessage = "Address cannot exceed 500 characters.")]
    public required string Address { get; set; }
}
