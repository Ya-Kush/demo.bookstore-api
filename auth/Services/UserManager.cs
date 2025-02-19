using Auth.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Auth.Services;

using UserOnlyStore = UserOnlyStore<User, UserContext, Guid, IdentityUserClaim<Guid>, IdentityUserLogin<Guid>, IdentityUserToken<Guid>>;

public sealed class UserManager(
        IUserStore<User> store,
        IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<User> passwordHasher,
        IEnumerable<IUserValidator<User>> userValidators,
        IEnumerable<IPasswordValidator<User>> passwordValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        IServiceProvider services,
        ILogger<UserManager<User>> logger) : UserManager<User>(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
{
    static readonly string RefreshProviderName = RefreshTokenProvider.ProviderName;
    static readonly string RefreshLoginProviderName = RefreshProviderName + "LoginProbider";
    static readonly string JwtBearerProviderName = JwtBearerTokenProvider.ProviderName;

    public new UserOnlyStore Store => (base.Store as object as UserOnlyStore)!;
    public UserContext Context => Store.Context;


    public Task<string> GenerateJwtBearerAccessToken(User user)
        => GenerateUserTokenAsync(user, JwtBearerProviderName, string.Empty);

    public Task<string> GenerateRefreshTokenAsync(User user, CancellationToken cancel = default)
        => GenerateUserTokenAsync(user, RefreshProviderName, string.Empty);

    public async Task<string> GenerateUpdatedRefreshTokenAsync(User user, CancellationToken cancel = default)
    {
        var token = await GenerateRefreshTokenAsync(user, cancel);
        await UpdateRefreshTokenAsync(user, token, cancel);
        return token;
    }

    public async Task<string> UpdateRefreshTokenAsync(User user, string token, CancellationToken cancel = default)
    {
        var userToken = await Context.UserTokens.FirstOrDefaultAsync(t => t.UserId == user.Id && t.Name == RefreshProviderName, cancel);
        if (userToken is null) Context.Add(userToken = new()
        {
            UserId = user.Id,
            LoginProvider = RefreshLoginProviderName,
            Name = RefreshProviderName
        });
        userToken.Value = token;
        await Context.SaveChangesAsync(cancel);
        return token;
    }

    public async Task<User?> FindByRefreshTokenAsync(string token, CancellationToken cancel = default)
    {
        var userToken = await Context.UserTokens
            .Join(Context.Users,
                t => t.UserId,
                u => u.Id,
                (token, user) => new { User = user, Token = token })
            .FirstOrDefaultAsync(ut => ut.Token.Name == RefreshProviderName && ut.Token.Value == token, cancel);
        return userToken?.User;
    }
}