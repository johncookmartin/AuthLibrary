using AuthLibrary.Data.Entities;

namespace AuthLibrary.DTOs;

public class AddProviderRequestDto
{
    public AuthUser User { get; init; } = null!;
    public string Provider { get; init; } = string.Empty;
    public string ProviderKey { get; init; } = string.Empty;
    public string? Name { get; init; }
}
