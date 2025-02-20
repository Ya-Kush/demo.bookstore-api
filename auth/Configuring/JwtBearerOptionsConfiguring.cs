using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Configuring;

static class JwtBearerOptionsConfiguring
{
    public static void Configure(this JwtBearerOptions opts, IConfiguration conf, IHostEnvironment env)
    {
        opts.RequireHttpsMetadata = env.IsDevelopment() is false;
        opts.TokenValidationParameters = new()
        {
            ValidIssuer = conf["Jwt:Issuer"],
            ValidAudience = conf["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(conf["Jwt:Key"]!)),
            ValidateIssuerSigningKey = true
        };
    }
}