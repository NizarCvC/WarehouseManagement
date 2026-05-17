using WarehouseCore.enums;

namespace WarehouseCore.Entities;

public class User
{
    public int UserID { get; init; }
    public required string Name { get; init; }
    public required string Username { get; init; }
    public required string Email { get; init; }
    public required string PasswordHash { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public int RoleID { get; init; }
    public enRole Role { get => (enRole)RoleID; }
    public string? RefreshToken { get; init; }
    public DateTime? RefreshTokenExpiryTime { get; init; }
}
