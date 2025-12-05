namespace AuthLibrary.DTOs;

public class LoginRequestDto
{
    public string Email { get; init; } = string.Empty;
    public string? Password { get; init; }
    public string? Provider { get; init; }
    public string? ProviderKey { get; init; }
    public string? Name { get; init; }
    public bool IsUsingProvider()
    {
        return !string.IsNullOrWhiteSpace(Provider) && !string.IsNullOrWhiteSpace(ProviderKey);
    }
}
