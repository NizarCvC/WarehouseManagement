using System.ComponentModel.DataAnnotations;

namespace WarehouseCore.DTOs.CreateDTOs;

public class CreateUserDto
{
    [Required(ErrorMessage = "The name is required")]
    [MaxLength(100)]
    public required string Name { get; set; }
    [Required(ErrorMessage = "The username is required")]
    [MaxLength(100)]
    public required string Username { get; set; }
    [Required(ErrorMessage = "The email is required")]
    [EmailAddress(ErrorMessage = "The email is not valid")]
    [MaxLength(100)]
    public required string Email { get; set; }
    [Required(ErrorMessage = "The password is required")]
    [MaxLength(100)]
    public required string Password { get; set; }
    [Required(ErrorMessage = "The role id is required")]
    [Range(1, 5, ErrorMessage = "The role id is not valid")]
    public int RoleID { get; set; }
}
