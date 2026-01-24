using AuthLibrary.Data.Entities;
using AuthLibrary.DTOs.Login;
using AuthLibrary.DTOs.Provider;
using AuthLibrary.DTOs.Register;

namespace AuthLibrary.Services.Interfaces;

public interface IUserService
{
    Task<LoginResult> LoginUser(LoginRequest request);
    Task<RegisterResult> RegisterUser(RegisterRequest request);
    Task<AuthUser?> GetUserByEmailAsync(string email);
    Task<AddProviderResult> AddProvider(AddProviderRequest request);
}
