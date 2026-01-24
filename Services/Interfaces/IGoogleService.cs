using AuthLibrary.DTOs;

namespace AuthLibrary.Services.Interfaces;

public interface IGoogleService
{
    Task<GoogleLoginResultDto> LoginWithGoogleAsync(GoogleRequestDto googleRequest);
    string GenerateInitialsFromName(string? familyName, string? givenName);
}