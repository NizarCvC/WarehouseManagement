using WarehouseCore.DTOs.AuthDTOs;
namespace WarehouseServices.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto, CancellationToken ct);
}
