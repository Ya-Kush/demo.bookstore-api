using App.Data;
using App.EndpointModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static App.Endpoints.BookHandlers;

namespace App.Endpoints;

public static partial class MapEndpointsExtensions
{
    static IEndpointRouteBuilder MapBooks(this IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("/books");

        group.MapGet("", GetBooks).WithName(nameof(GetBooks));

        group.MapGet("/{id:guid}", GetBook).WithName(nameof(GetBook));

        group.MapGet("/{id:guid}/authors", GetBookAuthors).WithName(nameof(GetBookAuthors));


        group.MapPost("", PostBook).WithName(nameof(PostBook));

        group.MapPut("/{id:guid}", PutBook).WithName(nameof(PutBook));

        group.MapPatch("/{id:guid}", PatchBook).WithName(nameof(PatchBook));

        return routeBuilder;
    }
}

public static class BookHandlers
{
    public static object GetBooks(BookstoreDbContext db, LinkGenerator lg)
    {
        return db.Books
            .AsNoTracking()
            .AsEnumerable()
            .Select(x => x
                .ToGetBook()
                .WithLinks(new
                {
                    self = lg.GetPathByName("GetBook", new { x.Id }),
                    books = lg.GetPathByName("GetBooks"),
                    authors = lg.GetPathByName("GetBookAuthors", new { x.Id })
                }));
    }

    public static object? GetBook(Guid id, BookstoreDbContext db, LinkGenerator lg)
    {
        return db.Books
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == id)?
            .ToGetBook()
            .WithLinks(new
            {
                self = lg.GetPathByName("GetBook", new { id }),
                books = lg.GetPathByName("GetBooks"),
                bookAuthors = lg.GetPathByName("GetBookAuthors", new { id })
            });
    }

    public static object? GetBookAuthors(Guid id, BookstoreDbContext db, LinkGenerator lg)
    {
        return db.Books
            .AsNoTracking()
            .Include(x => x.Authors)
            .FirstOrDefault(x => x.Id == id)?.Authors
            .Select(x => x
                .ToGetAuthor()
                .WithLinks(new
                {
                    self = lg.GetPathByName("GetBookAuthors", new { x.Id }),
                    books = lg.GetPathByName("GetBooks"),
                    book = lg.GetPathByName("GetBook", new { x.Id })
                }));
    }

    public static object PostBook([FromBody]PostBook newBook, BookstoreDbContext db, LinkGenerator lg)
    {
        var book = db.Books.Add(newBook.ToBook()).Entity;
        db.SaveChanges();

        return book.ToGetBook()
            .WithLinks(new
            {
                self = lg.GetPathByName("GetBook", new { book.Id }),
                books = lg.GetPathByName("GetBooks"),
                bookAuthors = lg.GetPathByName("GetBookAuthors", new { book.Id })
            });
    }

    public static object? PutBook(Guid id, [FromBody]PostBook newBook, BookstoreDbContext db)
    {
        var book = db.Books.FirstOrDefault(x => x.Id == id);
        if (book is null) return null;

        book.Update(newBook);
        db.SaveChanges();
        return book?.ToGetBook();
    }

    public static object? PatchBook(Guid id, [FromBody]PatchBook modified, BookstoreDbContext db)
    {
        var book = db.Books.FirstOrDefault(x => x.Id == id);
        if (book is null) return null;

        book.Update(modified);
        db.SaveChanges();
        return book?.ToGetBook();
    }
}
