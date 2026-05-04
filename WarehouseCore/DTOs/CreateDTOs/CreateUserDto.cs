using System.ComponentModel.DataAnnotations;

namespace WarehouseCore.DTOs.CreateDTOs;

public class CreateUserDto
{
    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }
    [Required]
    [MaxLength(100)]
    public required string Username { get; set; }
    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public required string Email { get; set; }
    [Required]
    [MaxLength(100)]
    public required string Password { get; set; }
    [Required]
    public int RoleID { get; set; }
}
