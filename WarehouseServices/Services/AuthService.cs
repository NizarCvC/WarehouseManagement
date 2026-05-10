using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WarehouseCore.DTOs.AuthDTOs;
using WarehouseCore.Entities;
using WarehouseCore.enums;
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

        if (userInfo is null || !userInfo.IsActive || !Hashing.VerifyPassword(loginDto.Password, userInfo.PasswordHash))
            throw new UnauthorizedException("Invalid username or password");

        return await GenerateTokensAndSaveAsync(userInfo, ct);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto requestDto, CancellationToken ct)
    {
        var principal = GetPrincipalFromExpiredToken(requestDto.AccessToken);
        var username = principal.Identity?.Name;

        if (string.IsNullOrEmpty(username))
            throw new UnauthorizedException("Invalid access token");

        var userInfo = await userRepository.GetByUsernameAsync(username, ct);

        if (userInfo is null || 
            userInfo.RefreshToken != requestDto.RefreshToken || 
            userInfo.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new UnauthorizedException("Invalid or expired refresh token");
        }

        return await GenerateTokensAndSaveAsync(userInfo, ct);
    }

    private async Task<AuthResponseDto> GenerateTokensAndSaveAsync(User user, CancellationToken ct)
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
            new Claim(ClaimTypes.Role, user.Role.ToRoleName() ?? "User"),
            new Claim("full_name", user.Name)
        };

        var descriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expires,
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                SecurityAlgorithms.HmacSha256
            )
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(descriptor);
        var accessToken = tokenHandler.WriteToken(securityToken);

        var refreshToken = GenerateSecureRandomString();
        var refreshExpiresDays = double.Parse(jwtSettings["RefreshTokenExpirationInDays"]!);
        var refreshTokenExpiryTime = DateTime.UtcNow.AddDays(refreshExpiresDays);

        await userRepository.UpdateRefreshTokenAsync(user.UserID, refreshToken, refreshTokenExpiryTime, ct);

        return new AuthResponseDto()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Expires = expires
        };
    }

    private string GenerateSecureRandomString()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = configuration["Jwt:Key"]!;

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }
}