namespace AuthLibrary.DTOs;

public sealed class RefreshTokenResultDto
{
    public bool Succeeded { get; init; }
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public string ErrorMessage { get; init; } = string.Empty;

    public static RefreshTokenResultDto Success(string accessToken, string refreshToken) =>
        new() { Succeeded = true, AccessToken = accessToken, RefreshToken = refreshToken };
    public static RefreshTokenResultDto Failure(string errorMessage) =>
        new() { Succeeded = false, ErrorMessage = errorMessage };
}
