namespace AuthLibrary.Settings;

public class JwtSettings
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public int ExpirationInMinutes { get; set; } = 2;
    public int RefreshExpirationInDays { get; set; } = 7;
}
