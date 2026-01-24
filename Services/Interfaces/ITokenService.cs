using AuthLibrary.Data.Entities;
using AuthLibrary.DTOs;

namespace AuthLibrary.Services.Interfaces;

public interface ITokenService
{
    Task<string> GenerateRefreshTokenAsync(AuthUser user);
    Task<string> GenerateTokenAsync(AuthUser user, IList<string> roles);
    Task<RefreshTokenResultDto> RefreshTokenAsync(string refreshToken);
    Task<RefreshTokenValidationResultDto> ValidateRefreshTokenAsync(string refreshToken);
    Task<bool> RevokeRefreshTokensAsync(Guid userId);
}
