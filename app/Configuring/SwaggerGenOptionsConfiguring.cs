using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace App.Configuring;

static class SwaggerGenOptionsConfiguring
{
    public static void Configure(this SwaggerGenOptions opts)
    {
        var scheme = JwtBearerDefaults.AuthenticationScheme;
        var securityScheme = new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Description = "Authorization header using the JWT Bearer scheme",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = scheme,
            BearerFormat = "JWT",
        };
        var securityRequirement = new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = scheme
                    }
                },
                []
            }
        };

        opts.AddSecurityDefinition(scheme, securityScheme);
        opts.AddSecurityRequirement(securityRequirement);
    }
}