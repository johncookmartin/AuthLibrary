using AuthLibrary.Data;
using AuthLibrary.Data.Entities;
using AuthLibrary.Services.Interfaces;
using AuthLibrary.Settings;
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

    public string GenerateToken(AuthUser user, IList<string> roles)
    {
        SymmetricSecurityKey signingKey = new(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        SigningCredentials credentials = new(signingKey, SecurityAlgorithms.HmacSha256);

        List<Claim> claims =
        [
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                ..roles.Select(r => new Claim(ClaimTypes.Role, r))
        ];

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

    public async Task<string> GenerateRefreshToken(AuthUser user)
    {
        string refreshToken = await GenerateRefreshTokenHandler(user);
        await _context.SaveChangesAsync();

        return refreshToken;
    }

    private async Task<string> GenerateRefreshTokenHandler(AuthUser user)
    {
        await RevokeRefreshTokens(user.Id);
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

    public async Task<RefreshTokenResult> RefreshToken(string refreshToken)
    {
        RefreshToken? existingToken = await _context.RefreshTokens
                        .Include(r => r.User)
                        .FirstOrDefaultAsync(r => r.Token == refreshToken && r.DeletedAtUtc == null);
        if (existingToken == null || existingToken.ExpiresOnUtc < DateTime.UtcNow)
        {
            return RefreshTokenResult.Failure("Invalid or expired refresh token.");
        }
        var roles = await _userManager.GetRolesAsync(existingToken.User);

        string accessToken = GenerateToken(existingToken.User, roles);
        string newRefreshToken = await GenerateRefreshTokenHandler(existingToken.User);

        await _context.SaveChangesAsync();

        return RefreshTokenResult.Success(accessToken, newRefreshToken);
    }

    public async Task<bool> RevokeRefreshTokens(Guid userId)
    {
        await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.DeletedAtUtc == null)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(rt => rt.DeletedAtUtc, DateTime.UtcNow));
        return true;
    }
}
