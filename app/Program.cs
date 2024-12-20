using App.Data;
using App.Endpoints;
using App.Middlewares;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

#region Service Configuration
var srvs = builder.Services;

srvs.AddProblemDetails();
srvs.AddExceptionHandler<GlobalExceptionHandlingMiddleware>();

srvs.AddDbContext<BookstoreDbContext>(opts => opts.UseInMemoryDatabase("Bookstore"));
srvs.AddBookstoreEndpointServices();

srvs.AddEndpointsApiExplorer();
srvs.AddSwaggerGen();
#endregion Service Configuration

#region Application Configuration
var app = builder.Build();

app.UseExceptionHandler();

app.PopulateBookstore();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger().UseSwaggerUI();

    app.MapGet("", _ => throw new Exception("Test Throwing"));
}

app.MapBookstoreEndpoints("/api/v1");

app.Run();
#endregion Application Configuration
