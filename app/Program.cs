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
}

app.MapGroup("/api/v1").MapBookstoreEndpoints();
#endregion Application Configuration

app.Map("", (IEnumerable<EndpointDataSource> sources) =>
    sources.Select(s => new
    {
        dataSource = s.GetType().Name,
        endpoints = s.Endpoints.Select(e => e.DisplayName)
    }));

app.Run();
