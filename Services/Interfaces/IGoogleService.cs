using AuthLibrary.DTOs.Google;

namespace AuthLibrary.Services.Interfaces;

public interface IGoogleService
{
    Task<GoogleLoginResult> LoginWithGoogleAsync(GoogleRequest googleRequest);
    string GenerateInitialsFromName(string? familyName, string? givenName);
}