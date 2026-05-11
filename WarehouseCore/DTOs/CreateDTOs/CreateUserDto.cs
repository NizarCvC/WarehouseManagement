using System.ComponentModel.DataAnnotations;
using WarehouseCore.enums;

namespace WarehouseCore.DTOs.CreateDTOs;

public class CreateUserDto
{
    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(100)]
    public required string Name { get; set; }
    [Required(ErrorMessage = "Username is required.")]
    [MaxLength(100)]
    public required string Username { get; set; }
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email is not valid.")]
    [MaxLength(100)]
    public required string Email { get; set; }
    [Required(ErrorMessage = "Password is required.")]
    [MaxLength(100)]
    public required string Password { get; set; }
    [Required(ErrorMessage = "Role ID is required.")]
    [Range(1, 5, ErrorMessage = "Role ID is not valid.")]
    public required enRole RoleID { get; set; }
}
