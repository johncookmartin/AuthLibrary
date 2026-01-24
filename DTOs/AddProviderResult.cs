namespace AuthLibrary.DTOs;

public sealed class AddProviderResult
{
    public bool Succeeded { get; init; }
    public IEnumerable<string> Errors { get; init; } = Array.Empty<string>();
    public static AddProviderResult Success() =>
        new() { Succeeded = true };
    public static AddProviderResult Failure(IEnumerable<string> errors) =>
        new() { Succeeded = false, Errors = errors };
}
