using System.Security.Claims;
using System.Text;
using Auth.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Claims = System.Collections.Generic.Dictionary<string, object>;

namespace Auth.Services;

public readonly record struct JwtBearerToken(string AccessToken, string RefreshToken, string Type = "Bearer");
public sealed class JwtBearerGeneration(UserManager<User> um, IConfiguration conf)
{
    public async Task<JwtBearerToken> CreateAsync(User user)
    {
        var secretKey = Encoding.UTF8.GetBytes(conf["Jwt:Secret"]!);
        var securityKey = new SymmetricSecurityKey(secretKey);

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var subject = new ClaimsIdentity((await um.GetClaimsAsync(user)).Concat(
        [
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Name, user.UserName!),
        ]));

        var purpose = "purpose";
        var accessTokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = subject,
            IssuedAt = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddMinutes(15),
            Issuer = conf["Jwt:Issuer"],
            Audience = conf["Jwt:Audience"],
            SigningCredentials = credentials,
            Claims = new Claims([new(purpose, "access")]),
        };
        var refreshTokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = subject,
            IssuedAt = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddDays(2),
            Issuer = conf["Jwt:Issuer"],
            Audience = conf["Jwt:Audience"],
            SigningCredentials = credentials,
            Claims = new Claims([new(purpose, "refresh")]),
        };

        var jwtHandler = new JsonWebTokenHandler();

        return new(AccessToken: jwtHandler.CreateToken(accessTokenDescriptor), RefreshToken: jwtHandler.CreateToken(refreshTokenDescriptor));
    }
}