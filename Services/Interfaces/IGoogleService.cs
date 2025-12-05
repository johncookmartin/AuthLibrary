using AuthLibrary.DTOs;
using Google.Apis.Auth;

namespace AuthLibrary.Services.Interfaces;

public interface IGoogleService
{
    Task<GoogleLoginResult> LoginWithGoogleAsync(GoogleRequestDto googleRequest);
    string GenerateInitialsFromName(string familyName, string givenName);
}

public sealed class GoogleLoginResult
{
    public bool Succeeded { get; init; }
    public IEnumerable<string>? Errors { get; init; } = Array.Empty<string>();
    public GoogleJsonWebSignature.Payload? Payload { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public static GoogleLoginResult Success(GoogleJsonWebSignature.Payload payload, string displayName) =>
        new() { Succeeded = true, Payload = payload, DisplayName = displayName };
    public static GoogleLoginResult Failure(IEnumerable<string> errors) =>
        new()
        {
            Succeeded = false,
            Errors = errors
        };
    public static GoogleLoginResult Failure(string error) =>
        new() { Succeeded = false, Errors = new[] { error } };
}