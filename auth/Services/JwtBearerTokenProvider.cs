using System.Security.Claims;
using System.Text;
using Auth.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Services;
#warning It would be great if you could reuse TokenValidationParameters from JwtBearerOptionsConfiguring
public sealed class JwtBearerTokenProvider(IConfiguration conf) : IUserTwoFactorTokenProvider<User>
{
    public static readonly string ProviderName = "Bearer";

    public string Issuer => conf["Jwt:Issuer"] ?? throw new InvalidOperationException();
    public string Audience => conf["Jwt:Audience"] ?? throw new InvalidOperationException();
    SecurityKey SecurityKey => _securityKey ??= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(conf["Jwt:Key"]!));
    SecurityKey? _securityKey;

    public async Task<string> GenerateAsync(string _, UserManager<User> manager, User user)
    {
        var subject = new ClaimsIdentity(
        [
            new(JwtRegisteredClaimNames.Name, user.UserName!),
            ..await manager.GetClaimsAsync(user),
        ]);

        var accessTokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = subject,
            IssuedAt = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddMinutes(15),
            Issuer = Issuer,
            Audience = Audience,
            SigningCredentials = new(SecurityKey, SecurityAlgorithms.HmacSha256),
        };

        return new JsonWebTokenHandler().CreateToken(accessTokenDescriptor);
    }

    public async Task<bool> ValidateAsync(string purpose, string token, UserManager<User> manager, User user)
    {
        var data = token.Split('.').Select(Base64UrlEncoder.Decode).ToArray();

        var tvp = new TokenValidationParameters
        {
            ValidIssuer = Issuer,
            ValidAudience = Audience,
            IssuerSigningKey = SecurityKey,
            ValidateIssuerSigningKey = true
        };

        var jwtHandler = new JsonWebTokenHandler();
        var res = await jwtHandler.ValidateTokenAsync(token, tvp);

        return res.IsValid;
    }

    public Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<User> manager, User user) => Task.FromResult(false);
}