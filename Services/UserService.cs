using AuthLibrary.Data;
using AuthLibrary.Data.Entities;
using AuthLibrary.DTOs.Login;
using AuthLibrary.DTOs.Provider;
using AuthLibrary.DTOs.Register;
using AuthLibrary.Services.Interfaces;
using AuthLibrary.Settings.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace AuthLibrary.Services;

public class UserService : IUserService
{
    private readonly AuthDbContext _context;
    private readonly UserManager<AuthUser> _userManager;
    private readonly IAuthenticationSchemeProvider _schemeProvider;

    public UserService(AuthDbContext context, UserManager<AuthUser> userManager, IAuthenticationSchemeProvider schemeProvider)
    {
        _context = context;
        _userManager = userManager;
        _schemeProvider = schemeProvider;
    }

    public async Task<RegisterResult> RegisterUser(RegisterRequest request)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        var user = new AuthUser
        {
            UserName = request.Email,
            Email = request.Email,
            DisplayName = request.Name ?? request.Email
        };

        if (!request.IsUsingProvider() && string.IsNullOrWhiteSpace(request.Password))
        {
            return RegisterResult.Failure(new[] { "Password is required." });
        }

        IdentityResult identityResult = request.IsUsingProvider() ? await _userManager.CreateAsync(user) : await _userManager.CreateAsync(user, request.Password!);
        if (!identityResult.Succeeded)
        {
            var errors = string.Join(", ", identityResult.Errors.Select(e => e.Description));
            transaction.Rollback();
            return RegisterResult.Failure(new[] { errors });
        }

        IdentityResult roleResult = await _userManager.AddToRoleAsync(user, AuthRoles.User);
        if (!roleResult.Succeeded)
        {
            var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
            transaction.Rollback();
            return RegisterResult.Failure(new[] { errors });
        }

        if (request.IsUsingProvider())
        {
            var addProviderResult = await AddProvider(new AddProviderRequest
            {
                User = user,
                Provider = request.Provider!,
                ProviderKey = request.ProviderKey!,
                Name = request.Name
            });
            if (!addProviderResult.Succeeded)
            {
                transaction.Rollback();
                return RegisterResult.Failure(addProviderResult.Errors);
            }
        }

        await transaction.CommitAsync();

        return RegisterResult.Success(user);
    }

    public async Task<LoginResult> LoginUser(LoginRequest request)
    {
        AuthUser? user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            return LoginResult.Failure(new[] { "Invalid email." });
        }

        if (!request.IsUsingProvider())
        {
            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return LoginResult.Failure(new[] { "Password is required." });
            }
            if (!await _userManager.CheckPasswordAsync(user, request.Password))
            {
                return LoginResult.Failure(new[] { "Invalid password." });
            }
        }
        else
        {
            var logins = await _userManager.GetLoginsAsync(user);
            bool providerExists = logins.Any(l => l.LoginProvider == request.Provider && l.ProviderKey == request.ProviderKey);
            if (!providerExists)
            {
                return LoginResult.Failure(new[] { "Provider login not found for this user." });
            }
        }

        var roles = await _userManager.GetRolesAsync(user);
        return LoginResult.Success(user, roles);

    }

    public async Task<AddProviderResult> AddProvider(AddProviderRequest request)
    {
        var schemes = await _schemeProvider.GetAllSchemesAsync();
        if (!schemes.Any(s => s.Name == request.Provider && s.DisplayName == request.Name))
        {
            var userLoginInfo = new UserLoginInfo(request.Provider, request.ProviderKey, request.Name ?? request.Provider);
            IdentityResult loginResult = await _userManager.AddLoginAsync(request.User, userLoginInfo);
            if (!loginResult.Succeeded)
            {
                var errors = string.Join(", ", loginResult.Errors.Select(e => e.Description));
                return AddProviderResult.Failure(new[] { errors });
            }
        }

        return AddProviderResult.Success();
    }

    public async Task<AuthUser?> GetUserByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }
}
