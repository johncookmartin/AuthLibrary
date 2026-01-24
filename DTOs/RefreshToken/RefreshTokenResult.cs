namespace AuthLibrary.DTOs.RefreshToken;

public sealed record RefreshTokenResult
{
    public bool Succeeded { get; init; }
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public string ErrorMessage { get; init; } = string.Empty;

    public static RefreshTokenResult Success(string accessToken, string refreshToken) =>
        new() { Succeeded = true, AccessToken = accessToken, RefreshToken = refreshToken };
    public static RefreshTokenResult Failure(string errorMessage) =>
        new() { Succeeded = false, ErrorMessage = errorMessage };
}
