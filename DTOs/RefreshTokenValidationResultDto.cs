using AuthLibrary.Data.Entities;

namespace AuthLibrary.DTOs;

public sealed class RefreshTokenValidationResultDto
{
    public bool Succeeded { get; init; }
    public RefreshToken Token { get; init; } = null!;
    public AuthUser User { get; init; } = null!;
    public IList<string> Roles { get; init; } = new List<string>();
    public string ErrorMessage { get; init; } = string.Empty;
    public static RefreshTokenValidationResultDto Success(RefreshToken token, AuthUser user, IList<string> roles) =>
        new() { Succeeded = true, Token = token, User = user, Roles = roles };
    public static RefreshTokenValidationResultDto Failure(string errorMessage) =>
        new() { Succeeded = false, ErrorMessage = errorMessage };
}
