using Microsoft.AspNetCore.Identity;

namespace Auth.Data;

public sealed class User : IdentityUser<Guid>
{
    public User(string? userName = null) : base(userName ?? $"user@{Guid.NewGuid()}")
    {
        Id = Guid.NewGuid();
        SecurityStamp = Guid.NewGuid().ToString();
    }
}