using App.Data;
using App.EndpointModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static App.Endpoints.AuthorHandlers;

namespace App.Endpoints;

public static partial class MapEndpointsExtensions
{
    static IEndpointRouteBuilder MapAuthors(this IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("/authors");

        group.MapGet("", GetAuthors).WithName(nameof(GetAuthors));

        group.MapGet("/{id:guid}", GetAuthor).WithName("GetAuthor");

        group.MapGet("/{id:guid}/books", GetAuthorBooks).WithName("GetAuthorBooks");


        group.MapPost("/{id:guid}/books", PostAuthorBook).WithName(nameof(PostAuthorBook));

        return routeBuilder;
    }
}

public static class AuthorHandlers
{
    public static object GetAuthors(BookstoreDbContext db, LinkGenerator lg)
    {
        return db.Authors
            .AsNoTracking()
            .AsEnumerable()
            .Select(x => x
                .ToGetAuthor()
                .WithLinks(new
                {
                    self = lg.GetPathByName("GetAuthor", new { x.Id }),
                    authors = lg.GetPathByName("GetAuthors"),
                    books = lg.GetPathByName("GetAuthorBooks", new { x.Id })
                }));
    }

    public static object? GetAuthor(Guid id, BookstoreDbContext db, LinkGenerator lg)
    {
        return db.Authors
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == id)?
            .ToGetAuthor()
            .WithLinks(new
            {
                self = lg.GetPathByName("GetAuthor", new { id }),
                authors = lg.GetPathByName("GetAuthors"),
                books = lg.GetPathByName("GetAuthorBooks", new { id })
            });
    }

    public static object? GetAuthorBooks(Guid id, BookstoreDbContext db, LinkGenerator lg)
    {
        return db.Authors
            .AsNoTracking()
            .Include(x => x.Books)
            .FirstOrDefault(x => x.Id == id)?.Books
            .Select(x => x
                .ToGetBook()
                .WithLinks(new
                {
                    self = lg.GetPathByName("GetAuthorBooks", new { x.Id }),
                    books = lg.GetPathByName("GetAuthors"),
                    book = lg.GetPathByName("GetAuthor", new { x.Id })
                }));
    }

    public static object? PostAuthorBook(Guid id, [FromBody]PostBook newBook, BookstoreDbContext db, LinkGenerator lg)
    {
        var author = db.Authors.FirstOrDefault(x => x.Id == id);
        if (author is null) return null;

        var book = db.Books.Add(newBook.ToBook()).Entity;
        author.AddBooks(book);

        db.SaveChanges();

        return book?
            .ToGetBook()
            .WithLinks(new
            {
                self = lg.GetPathByName("GetBook", new { book.Id }),
                books = lg.GetPathByName("GetAuthorBooks", new { id })
            });
    }
}
