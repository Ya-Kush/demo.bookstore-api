using App.Data;
using App.EndpointHandlers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var srvs = builder.Services;

srvs.AddDbContext<BookstoreDbContext>(opts => opts.UseInMemoryDatabase("Bookstore"));
srvs.AddEndpointHandlers();

srvs.AddEndpointsApiExplorer();
srvs.AddSwaggerGen();

var app = builder.Build();

app.PopulateBookstore();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger().UseSwaggerUI();
}

app.MapEndpointHandlers("/api/v1");

app.Run();
