using AuthLibrary.Data.Entities;

namespace AuthLibrary.DTOs.Provider;

public record AddProviderRequest
{
    public AuthUser User { get; init; } = null!;
    public string Provider { get; init; } = string.Empty;
    public string ProviderKey { get; init; } = string.Empty;
    public string? Name { get; init; }
}
