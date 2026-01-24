using AuthLibrary.Data;
using AuthLibrary.Data.Entities;
using AuthLibrary.DTOs;
using AuthLibrary.Services.Interfaces;
using AuthLibrary.Settings;
using AuthLibrary.Settings.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthLibrary.Services;

public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly AuthDbContext _context;
    private readonly UserManager<AuthUser> _userManager;

    public TokenService(IOptions<AuthSettings> authSettings, AuthDbContext context, UserManager<AuthUser> userManager)
    {
        JwtSettings? jwtSettings = authSettings.Value.Jwt
            ?? throw new InvalidOperationException("JWT settings are not configured.");
        _jwtSettings = jwtSettings;
        _context = context;
        _userManager = userManager;
    }

    private async Task<List<Claim>> BuildClaimsAsync(AuthUser user, IList<string> roles)
    {
        List<Claim> claims =
        [
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            ..roles.Select(r => new Claim(ClaimTypes.Role, r))
        ];

        string securityStamp = await _userManager.GetSecurityStampAsync(user);
        claims.Add(new Claim(JwtClaimNames.SecurityStamp, securityStamp));

        return claims;
    }

    public async Task<string> GenerateTokenAsync(AuthUser user, IList<string> roles)
    {
        SymmetricSecurityKey signingKey = new(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        SigningCredentials credentials = new(signingKey, SecurityAlgorithms.HmacSha256);

        List<Claim> claims = await BuildClaimsAsync(user, roles);

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
            SigningCredentials = credentials,
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience
        };

        var tokenHandler = new JsonWebTokenHandler();
        string accessToken = tokenHandler.CreateToken(tokenDescriptor);
        return accessToken;
    }

    public async Task<string> GenerateRefreshTokenAsync(AuthUser user)
    {
        string refreshToken = await GenerateRefreshTokenHandlerAsync(user);
        await _context.SaveChangesAsync();

        return refreshToken;
    }

    private async Task<string> GenerateRefreshTokenHandlerAsync(AuthUser user)
    {
        await RevokeRefreshTokensAsync(user.Id);
        RefreshToken refreshToken = new()
        {
            Id = Guid.NewGuid(),
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)),
            ExpiresOnUtc = DateTime.UtcNow.AddDays(_jwtSettings.RefreshExpirationInDays),
            UserId = user.Id
        };
        _context.RefreshTokens.Add(refreshToken);
        return refreshToken.Token;
    }

    public async Task<RefreshTokenValidationResult> ValidateRefreshTokenAsync(string refreshToken)
    {
        RefreshToken? existingToken = await _context.RefreshTokens
                        .Include(r => r.User)
                        .FirstOrDefaultAsync(r => r.Token == refreshToken && r.DeletedAtUtc == null);
        if (existingToken == null || existingToken.ExpiresOnUtc < DateTime.UtcNow || existingToken.User == null)
        {
            return RefreshTokenValidationResult.Failure("Invalid or expired refresh token.");
        }
        var roles = await _userManager.GetRolesAsync(existingToken.User);

        return RefreshTokenValidationResult.Success(existingToken, existingToken.User, roles);
    }

    public async Task<RefreshTokenResult> RefreshTokenAsync(string refreshToken)
    {
        RefreshTokenValidationResult validationResult = await ValidateRefreshTokenAsync(refreshToken);
        if (!validationResult.Succeeded)
        {
            return RefreshTokenResult.Failure(validationResult.ErrorMessage!);
        }

        string accessToken = await GenerateTokenAsync(validationResult.User, validationResult.Roles);
        string newRefreshToken = await GenerateRefreshTokenHandlerAsync(validationResult.User);

        await _context.SaveChangesAsync();

        return RefreshTokenResult.Success(accessToken, newRefreshToken);
    }

    public async Task<bool> RevokeRefreshTokensAsync(Guid userId)
    {
        await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.DeletedAtUtc == null)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(rt => rt.DeletedAtUtc, DateTime.UtcNow));
        return true;
    }
}
