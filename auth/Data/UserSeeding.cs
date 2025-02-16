using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Auth.Data;

static class UserSeeding
{
    public static async Task PopulateUserDbAsync(this IApplicationBuilder builder)
    {
        await using var scope = builder.ApplicationServices.CreateAsyncScope();
        using var um = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        if (await um.Users.AnyAsync()) return;

        var admin = new User
        {
            UserName = "SuperUser",
            Email = "superuser@mail.com",
        };

        await um.CreateAsync(admin, "!Pa55word");
    }
}