using WarehouseCore.Entities;
using WarehouseCore.enums;

namespace WarehouseCore.DTOs.ReadDTOs;

public class UserDto
{
    public int UserID { get; set; }
    public required string Name { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public bool IsActive { get; set; }
    public required string RoleName { get; set; } 
    public DateTime CreatedAt { get; set; }

    public static UserDto FromEntity(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user), "Cannot create a DTO from a null user");

        UserDto dto = new UserDto()
        {
            UserID = user.UserID,
            Name = user.Name,
            Username = user.Username,
            Email = user.Email,
            IsActive = user.IsActive,
            RoleName = user.Role.ToRoleName(),
            CreatedAt = user.CreatedAt
        };

        return dto;
    }

    public static List<UserDto> FromEntities(List<User> users)
    {
        if (users == null)
            throw new ArgumentNullException(nameof(users), "Cannot create DTOs from a null collection");

        return users.Select(FromEntity).ToList();
    }
}