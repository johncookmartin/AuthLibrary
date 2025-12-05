using AuthLibrary.Data.Entities;

namespace AuthLibrary.Services.Interfaces;

public interface ITokenService
{
    Task<string> GenerateRefreshToken(AuthUser user);
    string GenerateToken(AuthUser user, IList<string> roles);
    Task<RefreshTokenResult> RefreshToken(string refreshToken);
    Task<bool> RevokeRefreshTokens(Guid userId);
}

public sealed class RefreshTokenResult
{
    public bool Succeeded { get; init; }
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public string ErrorMessage { get; init; } = string.Empty;

    public static RefreshTokenResult Success(string accessToken, string refreshToken) =>
        new() { Succeeded = true, AccessToken = accessToken, RefreshToken = refreshToken };
    public static RefreshTokenResult Failure(string errorMessage) =>
        new() { Succeeded = false, ErrorMessage = errorMessage };
}
