namespace AuthLibrary.DTOs;

public class RegisterRequestDto
{
    public string Email { get; init; } = string.Empty;
    public string Initials { get; init; } = string.Empty;
    public string? Password { get; init; }
    public bool EnableNotifications { get; init; } = false;
    public string? Provider { get; init; }
    public string? ProviderKey { get; init; }
    public string? Name { get; init; }
    public bool IsUsingProvider()
    {
        return !string.IsNullOrWhiteSpace(Provider) && !string.IsNullOrWhiteSpace(ProviderKey);
    }
}
