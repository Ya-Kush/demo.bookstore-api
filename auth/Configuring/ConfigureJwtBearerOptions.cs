using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Configuring;

public class ConfigureJwtBearerOptions(
    IOptions<JwtOptions> jwtOptions,
    IHostEnvironment env,
    ILogger<ConfigureJwtBearerOptions> logger
) : IConfigureNamedOptions<JwtBearerOptions>
{
    public const string Name = JwtBearerDefaults.AuthenticationScheme;

    public void Configure(JwtBearerOptions options) => Configure(Name, options);
    public void Configure(string? name, JwtBearerOptions options)
    {
        if (name == Options.DefaultName)
        {
            logger.LogInformation("Called with default options name");
            return;
        }
        if (name is not Name)
        {
            logger.LogInformation("There was no match the name. Actual name: {name}", name);
            return;
        }

        var opts = jwtOptions.Value;
        options.RequireHttpsMetadata = env.IsDevelopment() is false;
        options.TokenValidationParameters = new()
        {
            ValidIssuer = opts.Issuer,
            ValidAudience = opts.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(opts.Key)),
            ValidateIssuerSigningKey = true
        };
    }
}