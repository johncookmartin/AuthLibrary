using AuthLibrary.DTOs;
using AuthLibrary.Services.Interfaces;
using AuthLibrary.Settings;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace AuthLibrary.Services;

public class GoogleService : IGoogleService
{
    private readonly GoogleSettings _googleSettings;
    private readonly IAuthenticationSchemeProvider _schemeProvider;

    public GoogleService(IOptions<AuthSettings> authSettings, IAuthenticationSchemeProvider schemeProvider)
    {
        GoogleSettings? googleSettings = authSettings.Value.Google
            ?? throw new InvalidOperationException("Google settings are not configured.");
        _googleSettings = authSettings.Value.Google;
        _schemeProvider = schemeProvider;
    }
    public async Task<GoogleLoginResult> LoginWithGoogleAsync(GoogleRequestDto request)
    {
        GoogleJsonWebSignature.Payload payload;
        try
        {
            var validationSettings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _googleSettings.ClientId }
            };

            payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, validationSettings);
        }
        catch (Exception)
        {
            return GoogleLoginResult.Failure("Invalid Google ID token.");
        }

        if (string.IsNullOrWhiteSpace(payload.Email))
        {
            return GoogleLoginResult.Failure("Google ID token does not contain an email.");
        }

        return GoogleLoginResult.Success(payload, GenerateDisplayName(payload));

    }

    public async Task<bool> IsGoogleLoginRegistered(string displayName)
    {
        var schemes = await _schemeProvider.GetAllSchemesAsync();
        return schemes.Any(s => s.Name == "Google" && s.DisplayName == displayName);
    }

    public string GenerateInitialsFromName(string familyName, string givenName)
    {
        string initials = string.Empty;
        if (!string.IsNullOrWhiteSpace(givenName))
        {
            initials += char.ToUpper(givenName[0]);
        }
        if (!string.IsNullOrWhiteSpace(familyName))
        {
            initials += char.ToUpper(familyName[0]);
        }

        int dashIndex = familyName.IndexOf('-');
        if (dashIndex > 0 && dashIndex < familyName.Length - 1)
        {
            initials += char.ToUpper(familyName[dashIndex + 1]);
        }
        return initials;
    }

    private string GenerateDisplayName(GoogleJsonWebSignature.Payload payload)
    {
        if (!string.IsNullOrWhiteSpace(payload.Name))
        {
            return payload.Name;
        }
        if (string.IsNullOrWhiteSpace(payload.GivenName) && string.IsNullOrWhiteSpace(payload.FamilyName))
        {
            return payload.Email ?? string.Empty;
        }
        return $"{payload.GivenName} {payload.FamilyName}";
    }
}
