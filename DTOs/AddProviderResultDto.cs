namespace AuthLibrary.DTOs;

public sealed class AddProviderResultDto
{
    public bool Succeeded { get; init; }
    public IEnumerable<string> Errors { get; init; } = Array.Empty<string>();
    public static AddProviderResultDto Success() =>
        new() { Succeeded = true };
    public static AddProviderResultDto Failure(IEnumerable<string> errors) =>
        new() { Succeeded = false, Errors = errors };
}
