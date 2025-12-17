using AuthLibrary.Data.Entities;

namespace AuthLibrary.Services.Interfaces;

public interface ITokenService
{
    Task<string> GenerateRefreshTokenAsync(AuthUser user);
    Task<string> GenerateTokenAsync(AuthUser user, IList<string> roles);
    Task<RefreshTokenResult> RefreshTokenAsync(string refreshToken);
    Task<RefreshTokenValidationResult> ValidateRefreshTokenAsync(string refreshToken);
    Task<bool> RevokeRefreshTokensAsync(Guid userId);
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

public sealed class RefreshTokenValidationResult
{
    public bool Succeeded { get; init; }
    public RefreshToken Token { get; init; } = null!;
    public AuthUser User { get; init; } = null!;
    public IList<string> Roles { get; init; } = new List<string>();
    public string ErrorMessage { get; init; } = string.Empty;
    public static RefreshTokenValidationResult Success(RefreshToken token, AuthUser user, IList<string> roles) =>
        new() { Succeeded = true, Token = token, User = user, Roles = roles };
    public static RefreshTokenValidationResult Failure(string errorMessage) =>
        new() { Succeeded = false, ErrorMessage = errorMessage };
}
