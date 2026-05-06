using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WarehouseCore.DTOs.AuthDTOs;
using WarehouseCore.Entities;
using WarehouseDataAccess.Interfaces;
using WarehouseServices.Exceptions;
using WarehouseServices.Interfaces;
using WarehouseServices.Security;

namespace WarehouseServices.Services;

public class AuthService(IConfiguration configuration, IUserRepository userRepository) : IAuthService
{
    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto, CancellationToken ct)
    {
        var userInfo = await userRepository.GetByUsernameAsync(loginDto.Username, ct);

        if (userInfo is null || !Hashing.VerifyPassword(loginDto.Password, userInfo.PasswordHash))
            throw new UnauthorizedException("Invalid username or password");

        return GenerateJwtToken(userInfo);
    }

    private AuthResponseDto GenerateJwtToken(User user)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = configuration["Jwt:Key"]!;
        
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];
        var expires = DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["TokenExpirationInMinutes"]!));

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role?.Name ?? "User"),
            new Claim("FullName", user.Name) 
        };

        var descriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expires,
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var securityToken = tokenHandler.CreateToken(descriptor);

        return new AuthResponseDto() {
            AccessToken = tokenHandler.WriteToken(securityToken),
            RefreshToken = "",
            Expires = expires
        };
    }
}
