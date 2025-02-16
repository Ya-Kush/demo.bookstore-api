using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace App.Configuring;

static class JwtBearerOptionsConfiguring
{
    public static void Configure(this JwtBearerOptions opts, IConfiguration conf, IHostEnvironment env)
    {
        opts.RequireHttpsMetadata = env.IsDevelopment() is false;
        opts.TokenValidationParameters = new()
        {
            ValidIssuer = conf["Jwt:Issuer"],
            ValidAudience = conf["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(conf["Jwt:Secret"]!)), // "RsaSecurityKey" for asymmetric
            ValidateIssuerSigningKey = true

            // Default:
            // ValidateIssuer = true,
            // ValidateAudience = true,
            // ValidateLifetime = true,
        };
    }
}