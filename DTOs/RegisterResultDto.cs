using AuthLibrary.Data.Entities;

namespace AuthLibrary.DTOs;

public sealed class RegisterResultDto
{
    public bool Succeeded { get; init; }
    public IEnumerable<string> Errors { get; init; } = Array.Empty<string>();
    public AuthUser? User { get; init; }

    public static RegisterResultDto Success(AuthUser user) =>
        new() { Succeeded = true, User = user };

    public static RegisterResultDto Failure(IEnumerable<string> errors) =>
        new() { Succeeded = false, Errors = errors };
}
