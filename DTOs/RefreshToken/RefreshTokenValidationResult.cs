using AuthLibrary.Data.Entities;

namespace AuthLibrary.DTOs.RefreshToken;

public sealed record RefreshTokenValidationResult
{
    public bool Succeeded { get; init; }
    public Data.Entities.RefreshToken Token { get; init; } = null!;
    public AuthUser User { get; init; } = null!;
    public IList<string> Roles { get; init; } = new List<string>();
    public string ErrorMessage { get; init; } = string.Empty;
    public static RefreshTokenValidationResult Success(Data.Entities.RefreshToken token, AuthUser user, IList<string> roles) =>
        new() { Succeeded = true, Token = token, User = user, Roles = roles };
    public static RefreshTokenValidationResult Failure(string errorMessage) =>
        new() { Succeeded = false, ErrorMessage = errorMessage };
}
