using Microsoft.AspNetCore.Identity;

namespace AuthLibrary.Data.Entities;

public class AuthUser : IdentityUser<Guid>
{
    public string DisplayName { get; set; } = string.Empty;
}
