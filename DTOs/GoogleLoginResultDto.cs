namespace AuthLibrary.DTOs;

public sealed class GoogleLoginResultDto
{
    public bool Succeeded { get; init; }
    public IEnumerable<string>? Errors { get; init; } = Array.Empty<string>();
    public string Email { get; init; } = string.Empty;
    public string? Name { get; init; }
    public string? FamilyName { get; init; }
    public string? GivenName { get; init; }
    public string? PictureUrl { get; init; }
    public string Subject { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public static GoogleLoginResultDto Success(Google.Apis.Auth.GoogleJsonWebSignature.Payload payload, string displayName) =>
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
    public static GoogleLoginResultDto Failure(IEnumerable<string> errors) =>
        new()
        {
            Succeeded = false,
            Errors = errors
        };
    public static GoogleLoginResultDto Failure(string error) =>
        new() { Succeeded = false, Errors = new[] { error } };
}
