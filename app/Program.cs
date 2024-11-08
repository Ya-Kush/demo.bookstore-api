using App.Endpoints;
using App.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var srvs = builder.Services;

srvs.AddDbContext<BookstoreDbContext>(opts => opts.UseInMemoryDatabase("Bookstore"));

srvs.AddEndpointsApiExplorer();
srvs.AddSwaggerGen();

var app = builder.Build();
app.PopulateBookstore();

if (app.Environment.IsDevelopment())
    app.UseSwagger().UseSwaggerUI();

app.MapBookstoreApi("/api/v1");

app.Run();
