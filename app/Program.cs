using App.Data;
using App.Handlers.Mapping;
using App.Middlewares;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var srvs = builder.Services;
{
    srvs.AddProblemDetails();
    srvs.AddExceptionHandler<GlobalExceptionHandlingMiddleware>();

    srvs.AddDbContext<BookstoreDbContext>(opts => opts.UseInMemoryDatabase("Bookstore"));
    srvs.AddBookstoreEndpointServices();

    srvs.AddEndpointsApiExplorer();
    srvs.AddSwaggerGen();
}


var app = builder.Build();
{
    app.UseExceptionHandler();

    app.PopulateBookstore();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.MapGroup("/api/v1").MapBookstoreEndpoints();

    app.Run();
}