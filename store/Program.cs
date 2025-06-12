using Store.Configuring;
using Store.Data;
using Store.Handlers.Mapping;
using Store.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

var bldr = WebApplication.CreateBuilder(args);
var conf = bldr.Configuration;
var env = bldr.Environment;

var srvs = bldr.Services;
{
    srvs.AddProblemDetails();
    srvs.AddExceptionHandler<GlobalExceptionHandlingMiddleware>();

    srvs.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts => opts.Configure(conf, env));
    srvs.AddAuthorization();

    srvs.AddDbContext<BookstoreDbContext>(opts => opts.UseInMemoryDatabase("Bookstore"));
    srvs.AddBookstoreEndpointServices();

    srvs.AddEndpointsApiExplorer();
    srvs.AddSwaggerGen(opts => opts.Configure());
}


var app = bldr.Build();
{
    app.UseExceptionHandler();

    await app.PopulateBookstoreAsync();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapGroup("/api/v1")
        .MapBookstoreEndpoints()
        .RequireAuthorization();

    app.Run();
}