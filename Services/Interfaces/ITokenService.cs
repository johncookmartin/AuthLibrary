using AuthLibrary.Data.Entities;
using AuthLibrary.DTOs.RefreshToken;

namespace AuthLibrary.Services.Interfaces;

public interface ITokenService
{
    Task<string> GenerateRefreshTokenAsync(AuthUser user);
    Task<string> GenerateTokenAsync(AuthUser user, IList<string> roles);
    Task<RefreshTokenResult> RefreshTokenAsync(string refreshToken);
    Task<RefreshTokenValidationResult> ValidateRefreshTokenAsync(string refreshToken);
    Task<bool> RevokeRefreshTokensAsync(Guid userId);
}
