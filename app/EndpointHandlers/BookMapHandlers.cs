using App.EndpointModels;
using App.EndpointModelValidators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace App.EndpointHandlers;

public static class BookMapHandlers
{
    public static RouteGroupBuilder MapBooks(this IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("/books");


        group.MapGetNamedAsHandler("", GetBooks);
        group.MapGetNamedAsHandler("/{id:guid}", GetBook);
        group.MapGetNamedAsHandler("/{id:guid}/authors", GetBookAuthors);

        group.MapPostNamedAsHandler("", PostBook);
        group.MapPutNamedAsHandler("/{id:guid}", PutBook);
        group.MapPatchNamedAsHandler("/{id:guid}", PatchBook);

        group.MapDeleteNamedAsHandler("/{id:guid}", DeleteBook);


        return group;
    }


    public static IResult GetBooks(EndpointHandlerContext context)
    {
        var (db, hc, lg) = (context.DbContext, context.HttpContext, context.LinkGenerator);

        return Ok(db.Books
            .AsNoTracking()
            .AsEnumerable()
            .Select(b => b
                .ToGetBook()
                .WithLinks(new
                {
                    self = lg.GetUriByName(hc, "GetBook", new { b.Id }),
                    authors = lg.GetUriByName(hc, "GetBookAuthors", new { b.Id }),
                })));
    }

    public static IResult GetBook(Guid id, EndpointHandlerContext context)
    {
        var (db, hc, lg) = (context.DbContext, context.HttpContext, context.LinkGenerator);

        var book = db.Books
            .AsNoTracking()
            .FirstOrDefault(b => b.Id == id);
        return book is null
            ? NotFound(book)
            : Ok(book
                .ToGetBook()
                .WithLinks(new
                {
                    self = lg.GetUriByName(hc, "GetBook", new { id }),
                    authors = lg.GetUriByName(hc, "GetBookAuthors", new { id }),
                }));
    }

    public static IResult GetBookAuthors(Guid id, EndpointHandlerContext context)
    {
        var (db, hc, lg) = (context.DbContext, context.HttpContext, context.LinkGenerator);

        var book = db.Books
            .AsNoTracking()
            .Include(b => b.Authors)
            .FirstOrDefault(b => b.Id == id);

        return book is null
            ? NotFound(book)
            : Ok(book.Authors
                .Select(a => a
                    .ToGetAuthor()
                    .WithLinks(new
                    {
                        self = lg.GetUriByName(hc, "GetAuthor", new { a.Id }),
                        books = lg.GetUriByName(hc, "GetAuthorBooks", new { a.Id }),
                    })));
    }


    public static IResult PostBook([FromBody]PostBook postBook, EndpointHandlerContext context)
    {
        var (db, hc, lg) = (context.DbContext, context.HttpContext, context.LinkGenerator);

        var validated = postBook.SimpleValidate();
        if (validated.IsValid is false) return BadRequest(new { validated.Errors });

        var book = db.Books.Add(postBook.ToBook()).Entity;
        db.SaveChanges();

        var createdAt = lg.GetUriByName(hc, "GetBook", new { book.Id });
        return Created(createdAt, book.ToGetBook()
            .WithLinks(new
            {
                self = createdAt,
                authors = lg.GetUriByName(hc, "GetBookAuthors", new { book.Id }),
            }));
    }

    public static IResult PutBook(Guid id, [FromBody]PutBook putBook, EndpointHandlerContext context)
    {
        var (db, hc, lg) = (context.DbContext, context.HttpContext, context.LinkGenerator);

        var validated = putBook.SimpleValidate();
        if (validated.IsValid is false) return BadRequest(new { validated.Errors });

        var book = db.Books.FirstOrDefault(b => b.Id == id);
        if (book is null)
        {
            book = db.Books.Add(putBook.ToBook()).Entity;
            db.SaveChanges();
            var createdAt = lg.GetUriByName(hc, "GetBook", new { book.Id });
            return Created(createdAt, book.ToGetBook()
                .WithLinks(new
                {
                    self = createdAt,
                    authors = lg.GetUriByName(hc, "GetBookAuthors", new { book.Id }),
                }));
        }
        else
        {
            book.Swap(putBook);
            db.SaveChanges();
            return Ok(book.ToGetBook()
                .WithLinks(new
                {
                    self = lg.GetUriByName(hc, "GetBook", new { book.Id }),
                    authors = lg.GetUriByName(hc, "GetBookAuthors", new { book.Id }),
                }));
        }
    }

    public static IResult PatchBook(Guid id, [FromBody]PatchBook patchBook, EndpointHandlerContext context)
    {
        var (db, hc, lg) = (context.DbContext, context.HttpContext, context.LinkGenerator);

        var validated = patchBook.SimpleValidate();
        if (validated.IsValid is false) return BadRequest(new { validated.Errors });

        var book = db.Books.FirstOrDefault(b => b.Id == id);
        if (book is null) return NotFound();

        book.Update(patchBook);
        db.SaveChanges();

        return Ok(book.ToGetBook()
            .WithLinks(new
            {
                self = lg.GetUriByName(hc, "GetBook", new { book.Id }),
                authors = lg.GetUriByName(hc, "GetBookAuthors", new { book.Id }),
            }));
    }


    public static IResult DeleteBook(Guid id, EndpointHandlerContext context)
    {
        var (db, hc, lg) = (context.DbContext, context.HttpContext, context.LinkGenerator);

        var book = db.Books.FirstOrDefault(b => b.Id == id);
        if (book is null) return NotFound(book);

        var res = db.Books.Remove(book);
        db.SaveChanges();

        return Ok(new { id });
    }
}
