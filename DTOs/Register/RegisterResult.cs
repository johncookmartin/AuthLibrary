using AuthLibrary.Data.Entities;

namespace AuthLibrary.DTOs.Register;

public sealed record RegisterResult
{
    public bool Succeeded { get; init; }
    public IEnumerable<string> Errors { get; init; } = Array.Empty<string>();
    public AuthUser? User { get; init; }

    public static RegisterResult Success(AuthUser user) =>
        new() { Succeeded = true, User = user };

    public static RegisterResult Failure(IEnumerable<string> errors) =>
        new() { Succeeded = false, Errors = errors };
}
