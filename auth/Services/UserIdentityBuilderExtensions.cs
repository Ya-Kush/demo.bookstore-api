using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Auth.Services;

public static class UserIdentityBuilderExtensions
{
    public static IdentityBuilder AddEntityFrameworkStores<TContext>(this IdentityBuilder identityBuilder, Action<DbContextOptionsBuilder> action)
            where TContext : DbContext
    {
        identityBuilder.Services.AddDbContext<TContext>(action);
        return identityBuilder.AddEntityFrameworkStores<TContext>();
    }
}