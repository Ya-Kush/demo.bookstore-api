using Auth.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Auth.Services;

public sealed class RefreshTokenProvider(UserContext userContext, IServiceProvider services) : IUserTwoFactorTokenProvider<User>
{
    public static readonly string ProviderName = "Refresh";
    public readonly UserContext UserContext = userContext;
    public readonly IServiceProvider Services = services;

    public async Task<string> GenerateAsync(string _, UserManager<User> manager, User user)
        => await manager.GenerateUserTokenAsync(user, TokenOptions.DefaultProvider, _);

    public async Task<bool> ValidateAsync(string purpose, string token, UserManager<User> manager, User user)
    {
        var savedToken = await UserContext.UserTokens.AsNoTracking()
            .FirstOrDefaultAsync(t => t.UserId == user.Id && t.Name == ProviderName);

        return savedToken?.Value == token;
    }

    public Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<User> manager, User user) => Task.FromResult(false);
}