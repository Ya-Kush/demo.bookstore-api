using App.Data;
using App.EndpointModels;
using Microsoft.AspNetCore.Mvc;

namespace App.Endpoints;

public static class EndpointMapperExtensions
{
    public static IEndpointRouteBuilder MapBookstoreApi(this IEndpointRouteBuilder routeBuilder, string prefix)
        => routeBuilder.MapGroup(prefix)
            .MapHomeGreeting("Hello! I'm happy to see you on my api!")
            .MapBooks()
            .MapAuthors()
            ;

    static IEndpointRouteBuilder MapHomeGreeting(this IEndpointRouteBuilder routeBuilder, string message)
    {
        routeBuilder.MapGet("/", () => message);
        return routeBuilder;
    }

    static IEndpointRouteBuilder MapBooks(this IEndpointRouteBuilder routeBuilder)
    {
        var booksGroup = routeBuilder.MapGroup("/books");

        booksGroup.MapGet("", (BookstoreDbContext db) => db.Books.Select(x => x.ToResponse()));
        booksGroup.MapGet("/{guid}", (Guid guid, BookstoreDbContext db) => db.Books.FirstOrDefault(x => x.Guid == guid)?.ToResponse());

        booksGroup.MapPost("", ([FromBody]ReceivedNewBook newBook, BookstoreDbContext db) =>
        {
            var book = db.Books.Add(newBook.ToBook()).Entity;
            db.SaveChanges();
            return book.ToResponse();
        });

        booksGroup.MapPatch("/{guid}", (Guid guid, [FromBody]ModifiedBook modified, BookstoreDbContext db) =>
        {
            var book = db.Books.FirstOrDefault(x => x.Guid == guid);
            if (book is null) return null;

            book.Modify(modified);
            db.SaveChanges();
            return book?.ToResponse();
        });

        return routeBuilder;
    }

    static IEndpointRouteBuilder MapAuthors(this IEndpointRouteBuilder routeBuilder)
    {
        var authorsGroup = routeBuilder.MapGroup("/authors");

        authorsGroup.MapGet("", (BookstoreDbContext db) => db.Authors.Select(x => x.ToResponse()));
        authorsGroup.MapGet("/{guid}", (Guid guid, BookstoreDbContext db) => db.Authors.FirstOrDefault(x => x.Guid == guid)?.ToResponse());

        return routeBuilder;
    }
}