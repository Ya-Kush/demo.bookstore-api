using App.Data;
using App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var srvs = builder.Services;

srvs.AddDbContext<BookstoreDbContext>(opts => opts.UseInMemoryDatabase("Bookstore").PopulateWithEverything());
#warning Seeding doesnt work.

srvs.AddEndpointsApiExplorer();
srvs.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment()) app.UseSwagger().UseSwaggerUI();

app.MapGet("/", () => "Hello!");

var api = app.MapGroup("api/v1/");
api.MapGet("", () => "Hello! I'm happy to see you on my api!");
api.MapGet("books/", (BookstoreDbContext db) => db.Books.Select(x => new { Id = x.Id.Value, Title = x.Title }));
api.MapGet("books/{id}", (Guid id, BookstoreDbContext db) =>
{
    var book = db.Books.FirstOrDefault(x => x.Id.Value == id);
    if (book is null) throw new KeyNotFoundException("boo");
    return book.Title;
});
api.MapPost("books/", ([FromBody]string title, BookstoreDbContext db) =>
{
    var book = db.Books.Add(Book.New(title, "e", ["a","b"], "p", "r", 0.0)).Entity;
    db.SaveChanges();
    return new { Id = book.Id.Value, book.Title };
});

app.Run();
