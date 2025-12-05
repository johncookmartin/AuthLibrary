using AuthLibrary.Data;
using AuthLibrary.Settings.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;

namespace AuthLibrary;

public static class InitializationExtensions
{
    public static async Task AuthInitialization(this WebApplication app)
    {

        if (app.Environment.IsDevelopment())
        {
            using IServiceScope scope = app.Services.CreateScope();
            AuthDbContext dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
            dbContext.Database.Migrate();

            RoleManager<IdentityRole<Guid>> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            foreach (var authRole in AuthRoles.All)
            {
                if (!await roleManager.RoleExistsAsync(authRole))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(authRole));
                }
            }
        }

        if (app.Environment.IsDevelopment())
        {
            // for testing auth
            app.MapGet("me", (ClaimsPrincipal claimsPrinciple) =>
            {
                return Results.Ok(claimsPrinciple.Claims.ToDictionary(c => c.Type, c => c.Value));
            })
            .RequireAuthorization(policy => policy.RequireRole(AuthRoles.User));
        }


        app.UseAuthentication();
        app.UseAuthorization();
    }
}
