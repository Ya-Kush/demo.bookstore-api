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
        ILogger<UserManager<User>> logger
    ) : UserManager<User>(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
{
    const string LoginProbider = "Application";
    const string RefreshProviderName = RefreshTokenProvider.Name;
    const string JwtBearerProviderName = JwtBearerTokenProvider.Name;

    public new UserOnlyStore Store => (base.Store as object as UserOnlyStore)!;
    public UserContext Context => Store.Context;


    public async Task<string> GenerateJwtBearerTokenAsync(User user) => await GenerateUserTokenAsync(user, JwtBearerProviderName, string.Empty);
    public async Task<string> GenerateRefreshTokenAsync(User user) => await GenerateUserTokenAsync(user, RefreshProviderName, string.Empty);

    public async Task<string> GenerateUpdatedRefreshTokenAsync(User user, CancellationToken cancel = default)
    {
        var token = await GenerateRefreshTokenAsync(user);
        await UpdateRefreshTokenAsync(user, token, cancel);
        return token;
    }

    public async Task<string> UpdateRefreshTokenAsync(User user, string token, CancellationToken cancel = default)
    {
        var userToken = await Context.UserTokens.FirstOrDefaultAsync(t => t.UserId == user.Id && t.Name == RefreshProviderName, cancel);
        if (userToken is null)
            Context.Add(userToken = new()
            {
                UserId = user.Id,
                LoginProvider = LoginProbider,
                Name = RefreshProviderName,
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