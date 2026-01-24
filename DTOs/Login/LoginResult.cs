using AuthLibrary.Data.Entities;

namespace AuthLibrary.DTOs.Login;

public sealed record LoginResult
{
    public bool Succeeded { get; init; }
    public IEnumerable<string> Errors { get; init; } = Array.Empty<string>();
    public AuthUser? User { get; init; }
    public IList<string>? Roles { get; init; }
    public static LoginResult Success(AuthUser user, IList<string> roles) =>
        new() { Succeeded = true, User = user, Roles = roles };
    public static LoginResult Failure(IEnumerable<string> errors) =>
        new() { Succeeded = false, Errors = errors };
}
