using Auth.Data;
using Auth.Services;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace Auth.Handlers;

sealed class Account
{
    static readonly string _authScheme = IdentityConstants.BearerScheme;

    public readonly record struct RegisterRequest(string? UserName, string Email, string Password);
    public static async Task<Results<Ok<JwtBearerToken>, ValidationProblem, EmptyHttpResult>> Register([FromBody]RegisterRequest req, SignInManager<User> sim, [FromServices]JwtBearerGeneration jwtGen)
    {
        var um = sim.UserManager;
        var (userName, email, password) = req;

        var user = await um.FindByEmailAsync(email);
        if (user is not null) return ValidationProblem(new Dictionary<string, string[]> {{ "Data", ["Invalid email or password"] }});

        user = new(userName) { Email = email };
        var iRes = await um.CreateAsync(user, password);
        if (iRes.Succeeded is false) return ValidationProblem(iRes.Errors.Select(e => new KeyValuePair<string, string[]>(e.Code, [e.Description])).ToDictionary());

        // sim.AuthenticationScheme = _authScheme;
        // await sim.SignInAsync(user, false, _authScheme);
        // return Empty;
        return Ok(await jwtGen.CreateAsync(user));
    }

    public static async Task<Ok<JwtBearerToken>> SuperLogIn(SignInManager<User> sim, [FromServices]JwtBearerGeneration jwtGen)
    {
        var su = (await sim.UserManager.FindByNameAsync("SuperUser"))!;
        var token = await jwtGen.CreateAsync(su);
        return Ok(token);
    }

    public readonly record struct LogInRequest(string Email, string Password);
    public static async Task<Results<Ok, BadRequest, EmptyHttpResult>> LogIn(LogInRequest req, SignInManager<User> sim)
    {
        var u = await sim.UserManager.FindByEmailAsync(req.Email);
        if (u is null) return BadRequest();

        var res = await sim.PasswordSignInAsync(u, req.Password, false, false);
        if (res.Succeeded is false) return BadRequest();

        return Empty;
    }

    public static async Task Refresh([FromBody]string refreshToken, SignInManager<User> sim)
    {
        throw new NotImplementedException();
    }
}