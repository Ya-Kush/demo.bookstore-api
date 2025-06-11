using Auth.Configuring;
using Auth.Data;
using Auth.Extensions;
using Auth.Handlers;
using Auth.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var bldr = WebApplication.CreateBuilder(args);
var conf = bldr.Configuration;
var env = bldr.Environment;

var srvs = bldr.Services;
{
    srvs.AddLogging();
    srvs.AddHttpContextAccessor();
    srvs.AddProblemDetails();

    srvs.ConfigureOptions<ConfigureJwtBearerOptions>();
    srvs.AddOptions<JwtOptions>()
        .BindConfiguration(JwtOptions.SectionName)
        .ValidateDataAnnotations()
        .ValidateOnStart();

    srvs.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
    srvs.AddAuthorization();

    srvs.AddIdentityCore<User>(opts =>
        {
            opts.User.RequireUniqueEmail = true;
            opts.Stores.SchemaVersion = IdentitySchemaVersions.Version2;
            opts.Stores.MaxLengthForKeys = Guid.Empty.ToString().Length;
            // opts.Stores.ProtectPersonalData = true;
        })
        .AddEntityFrameworkStores<UserContext>(opts => opts.UseInMemoryDatabase("Users"))
        .AddDefaultTokenProviders()
        .AddTokenProvider<JwtBearerTokenProvider>(JwtBearerTokenProvider.Name)
        .AddTokenProvider<RefreshTokenProvider>(RefreshTokenProvider.Name)
        .AddUserManager<UserManager>();

    srvs.AddEndpointsApiExplorer();
    srvs.AddSwaggerGen(opts => opts.Configure());
}

var app = bldr.Build();
{
    await app.PopulateUserDbAsync();

    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapGet("/", () => "Welcome to the auth service!").WithDisplayName("Greeting");
    app.MapGroup("api/account", group =>
        {
            group.MapGet("/claims", Claims.Get).RequireAuthorization();

            if (env.IsDevelopment())
                group.MapPost("/super-login", Account.SuperLogIn);

            group.MapPost("/register", Account.Register);
            group.MapPost("/login", Account.LogIn);
            group.MapPost("/refresh", Account.Refresh);
        }).WithTags("Account");

    app.Run();
}