using AuthLibrary.Data.Entities;
using AuthLibrary.DTOs;

namespace AuthLibrary.Services.Interfaces;

public interface IUserService
{
    Task<LoginResult> LoginUser(LoginRequestDto request);
    Task<RegisterResult> RegisterUser(RegisterRequestDto request);
    Task<AuthUser?> GetUserByEmailAsync(string email);
    Task<AddProviderResult> AddProvider(AddProviderRequestDto request);
}

public sealed class LoginResult
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

public sealed class RegisterResult
{
    public bool Succeeded { get; init; }
    public IEnumerable<string> Errors { get; init; } = Array.Empty<string>();
    public AuthUser? User { get; init; }

    public static RegisterResult Success(AuthUser user) =>
        new() { Succeeded = true, User = user };

    public static RegisterResult Failure(IEnumerable<string> errors) =>
        new() { Succeeded = false, Errors = errors };
}

public sealed class AddProviderResult
{
    public bool Succeeded { get; init; }
    public IEnumerable<string> Errors { get; init; } = Array.Empty<string>();
    public static AddProviderResult Success() =>
        new() { Succeeded = true };
    public static AddProviderResult Failure(IEnumerable<string> errors) =>
        new() { Succeeded = false, Errors = errors };

}
