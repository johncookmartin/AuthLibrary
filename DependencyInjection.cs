using AuthLibrary.Data;
using AuthLibrary.Data.Entities;
using AuthLibrary.Services;
using AuthLibrary.Services.Interfaces;
using AuthLibrary.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AuthLibrary;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthServices<TMigrationAssemblyMarker>(this IServiceCollection services, IConfiguration configuration)
    {
        AuthSettings? authSettings = configuration.GetSection("Auth").Get<AuthSettings>()
            ?? throw new InvalidOperationException("Auth settings are missing");
        JwtSettings? jwtSettings = authSettings.Jwt
            ?? throw new InvalidOperationException("JWT settings are missing");

        // Db context and Identity
        services.Configure<AuthSettings>(configuration.GetSection("Auth"));
        services.AddDbContext<AuthDbContext>((options) =>
        {
            string? connectionString = authSettings.ConnectionString
                ?? throw new InvalidOperationException("Auth connection string is not configured.");
            options.UseSqlServer(connectionString, sql =>
            {
                sql.MigrationsAssembly(typeof(TMigrationAssemblyMarker).Assembly.FullName);
            });
        });

        services.AddIdentityCore<AuthUser>()
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AuthDbContext>()
            .AddDefaultTokenProviders();

        // services
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IGoogleService, GoogleService>();

        // Authentication and Authorization
        var authBuilder = services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer((options) =>
            {
                options.TokenValidationParameters.ValidIssuer = jwtSettings.Issuer;
                options.TokenValidationParameters.ValidAudience = jwtSettings.Audience;
                options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
            });

        services.AddAuthorization();

        return services;
    }
}
