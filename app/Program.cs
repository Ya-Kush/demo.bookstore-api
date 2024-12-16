using App.Data;
using App.Endpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var srvs = builder.Services;

srvs.AddDbContext<BookstoreDbContext>(opts => opts.UseInMemoryDatabase("Bookstore"));
srvs.AddEndpointServices();

srvs.AddEndpointsApiExplorer();
srvs.AddSwaggerGen();

var app = builder.Build();

app.PopulateBookstore();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger().UseSwaggerUI();
}

app.MapEndpoints("/api/v1");

app.Run();
