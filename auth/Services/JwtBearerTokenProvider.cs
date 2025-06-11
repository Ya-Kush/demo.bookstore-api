using Auth.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Auth.Services;

public sealed class JwtBearerTokenProvider(IOptionsSnapshot<JwtBearerOptions> jwtBearerOpts) : IUserTwoFactorTokenProvider<User>
{
    public const string Name = JwtBearerDefaults.AuthenticationScheme;
    TokenValidationParameters TokenValidationParameters { get; } = jwtBearerOpts.Get(JwtBearerDefaults.AuthenticationScheme).TokenValidationParameters;
    static JsonWebTokenHandler Handler => new();

    public async Task<string> GenerateAsync(string _, UserManager<User> manager, User user)
    {
        var subject = new ClaimsIdentity(
        [
            new(JwtRegisteredClaimNames.Name, user.UserName!),
            ..await manager.GetClaimsAsync(user),
        ]);
        var signingKey = TokenValidationParameters.IssuerSigningKey;
        var issuer = TokenValidationParameters.ValidIssuer;
        var audience = TokenValidationParameters.ValidAudience;

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = subject,
            IssuedAt = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddMinutes(15),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new(signingKey, SecurityAlgorithms.HmacSha256),
        };

        return Handler.CreateToken(descriptor);
    }

    public async Task<bool> ValidateAsync(string _, string token, UserManager<User> manager, User user)
    {
        // var data = token.Split('.').Select(Base64UrlEncoder.Decode).ToArray();
        var res = await Handler.ValidateTokenAsync(token, TokenValidationParameters);
        return res.IsValid;
    }

    public Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<User> manager, User user) => Task.FromResult(false);
}