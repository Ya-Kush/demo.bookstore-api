using Auth.Data;
using Auth.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace Auth.Handlers;

sealed class Account
{
    public readonly record struct JwtAndRefreshResponse(string AccessToken, string RefreshToken){ public string Type { get; } = bearerProvider; }

    static readonly string bearerProvider = JwtBearerTokenProvider.Name;
    static readonly string defaultProvider = TokenOptions.DefaultProvider;


    public readonly record struct RegisterRequest(string? UserName, string Email, string Password);
    public static async Task<Results<Ok<JwtAndRefreshResponse>, ValidationProblem, EmptyHttpResult>> Register([FromBody]RegisterRequest req, UserManager um)
    {
        var (userName, email, password) = req;

        var user = await um.FindByEmailAsync(email);
        if (user is not null) return ValidationProblem(new Dictionary<string, string[]> {{ "Data", ["Invalid email or password"] }});

        user = new(userName) { Email = email };
        var iRes = await um.CreateAsync(user, password);
        if (iRes.Succeeded is false) return ValidationProblem(iRes.Errors.Select(e => new KeyValuePair<string, string[]>(e.Code, [e.Description])).ToDictionary());

        return Ok(await CreateTokensRespone(user, um));
    }

    public static async Task<Ok<JwtAndRefreshResponse>> SuperLogIn(UserManager um)
    {
        var user = (await um.FindByNameAsync("SuperUser"))!;
        return Ok(await CreateTokensRespone(user, um));
    }

    public readonly record struct LogInRequest(string Email, string Password);
    public static async Task<Results<Ok<JwtAndRefreshResponse>, BadRequest, EmptyHttpResult>> LogIn(LogInRequest req, UserManager um)
    {
        var (email, password) = req;
        var user = await um.FindByEmailAsync(email);
        if (user is null) return BadRequest();

        var irs = await Task.WhenAll(um.PasswordValidators.Select(v => v.ValidateAsync(um, user, password)));

        return irs.All(v => v.Succeeded) ? Ok(await CreateTokensRespone(user, um))
            : BadRequest();
    }

    public static async Task<Results<Ok<JwtAndRefreshResponse>, BadRequest, EmptyHttpResult>> Refresh([FromBody]string refreshToken, UserManager um)
    {
        var user = await um.FindByRefreshTokenAsync(refreshToken);
        return user is null ? BadRequest()
            : Ok(await CreateTokensRespone(user, um));
    }

    static async Task<JwtAndRefreshResponse> CreateTokensRespone(User user, UserManager um)
    {
        var accessToken = await um.GenerateJwtBearerTokenAsync(user);
        var refreshToken = await um.GenerateUpdatedRefreshTokenAsync(user);

        return new(accessToken, refreshToken);
    }
}