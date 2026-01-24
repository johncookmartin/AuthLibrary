using AuthLibrary.Data.Entities;
using AuthLibrary.DTOs;

namespace AuthLibrary.Services.Interfaces;

public interface IUserService
{
    Task<LoginResult> LoginUser(LoginRequestDto request);
    Task<RegisterResult> RegisterUser(RegisterRequestDto request);
    Task<AuthUser?> GetUserByEmailAsync(string email);
    Task<AddProviderResult> AddProvider(AddProviderRequestDto request);
}
