using AuthLibrary.Data.Entities;
using AuthLibrary.DTOs;

namespace AuthLibrary.Services.Interfaces;

public interface IUserService
{
    Task<LoginResultDto> LoginUser(LoginRequestDto request);
    Task<RegisterResultDto> RegisterUser(RegisterRequestDto request);
    Task<AuthUser?> GetUserByEmailAsync(string email);
    Task<AddProviderResultDto> AddProvider(AddProviderRequestDto request);
}
