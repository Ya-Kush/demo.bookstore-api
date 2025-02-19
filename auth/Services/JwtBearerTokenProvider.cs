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

    public async Task<string> GenerateAsync(string _, UserManager<User> manager, User user)
    {
        var secretKey = Encoding.UTF8.GetBytes(conf["Jwt:Key"]!);
        var securityKey = new SymmetricSecurityKey(secretKey);

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var subject = new ClaimsIdentity(
        [
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Name, user.UserName!),
            ..await manager.GetClaimsAsync(user),
        ]);

        var accessTokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = subject,
            IssuedAt = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddMinutes(15),
            Issuer = conf["Jwt:Issuer"],
            Audience = conf["Jwt:Audience"],
            SigningCredentials = credentials,
        };

        var jwtHandler = new JsonWebTokenHandler();

        return jwtHandler.CreateToken(accessTokenDescriptor);
    }

    public async Task<bool> ValidateAsync(string purpose, string token, UserManager<User> manager, User user)
    {
        var data = token.Split('.').Select(Base64UrlEncoder.Decode).ToArray();

        var jwtHandler = new JsonWebTokenHandler();
        var tvp = new TokenValidationParameters
        {
            ValidIssuer = conf["Jwt:Issuer"],
            ValidAudience = conf["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(conf["Jwt:Key"]!)),
            ValidateIssuerSigningKey = true
        };
        var can = jwtHandler.CanValidateToken;
        var res = await jwtHandler.ValidateTokenAsync(token, tvp);

        return res.IsValid;
    }

    public Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<User> manager, User user) => Task.FromResult(false);
}

public static class JwtBearerTokenProviderExtensions
{
    public static Task<string> GenerateJwtBearerAccessToken(this UserManager<User> manager, User user)
        => manager.GenerateUserTokenAsync(user, JwtBearerTokenProvider.ProviderName, string.Empty);
}