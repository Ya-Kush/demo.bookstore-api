using App.Data;
using App.Endpoints;
using App.Middlewares;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var srvs = builder.Services;

srvs.AddProblemDetails();
srvs.AddExceptionHandler<GlobalExceptionHandlingMiddleware>();

srvs.AddDbContext<BookstoreDbContext>(opts => opts.UseInMemoryDatabase("Bookstore"));
srvs.AddEndpointServices();

srvs.AddEndpointsApiExplorer();
srvs.AddSwaggerGen();

var app = builder.Build();

app.UseExceptionHandler();

app.PopulateBookstore();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger().UseSwaggerUI();

    app.MapGet("", _ => throw new Exception("Test Throwing"));
}

app.MapEndpoints("/api/v1");

app.Run();
