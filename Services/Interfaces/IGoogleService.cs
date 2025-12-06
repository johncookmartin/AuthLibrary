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
    public string? Email { get; init; }
    public string? Name { get; init; }
    public string? FamilyName { get; init; }
    public string? GivenName { get; init; }
    public string? PictureUrl { get; init; }
    public string? Subject { get; init; }
    public bool IsPayloadNull() => Email is null && Name is null && FamilyName is null && GivenName is null && PictureUrl is null && Subject is null;
    public string DisplayName { get; init; } = string.Empty;
    public static GoogleLoginResult Success(GoogleJsonWebSignature.Payload payload, string displayName) =>
        new()
        {
            Succeeded = true,
            DisplayName = displayName,
            Email = payload.Email,
            Name = payload.Name,
            FamilyName = payload.FamilyName,
            GivenName = payload.GivenName,
            PictureUrl = payload.Picture,
            Subject = payload.Subject
        };
    public static GoogleLoginResult Failure(IEnumerable<string> errors) =>
        new()
        {
            Succeeded = false,
            Errors = errors
        };
    public static GoogleLoginResult Failure(string error) =>
        new() { Succeeded = false, Errors = new[] { error } };
}