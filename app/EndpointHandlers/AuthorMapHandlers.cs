using App.EndpointModels;
using App.EndpointModelValidators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace App.EndpointHandlers;

public static class AuthorEndpoints
{
    public static RouteGroupBuilder MapAuthors(this IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("/authors");


        group.MapGetNamedAsHandler("", GetAuthors);
        group.MapGetNamedAsHandler("/{id:guid}", GetAuthor);
        group.MapGetNamedAsHandler("/{id:guid}/books", GetAuthorBooks);

        group.MapPostNamedAsHandler("/", PostAuthor);
        group.MapPutNamedAsHandler("/{id:guid}", PutAuthor);
        group.MapPatchNamedAsHandler("/{id:guid}", PatchAuthor);

        group.MapDeleteNamedAsHandler("/{id:guid}", DeleteAuthor);

        // group.MapPostNamedAsHandler("/{id:guid}/books", PostAuthorBook);

        return group;
    }


    public static IResult GetAuthors(EndpointHandlerContext context)
    {
        var (db, hc, lg) = (context.DbContext, context.HttpContext, context.LinkGenerator);

        return Ok(db.Authors
            .AsNoTracking()
            .AsEnumerable()
            .Select(a => a
                .ToGetAuthor()
                .WithLinks(new
                {
                    self = lg.GetUriByName(hc, "GetAuthor", new { a.Id }),
                    books = lg.GetUriByName(hc, "GetAuthorBooks", new { a.Id }),
                })));
    }

    public static IResult GetAuthor(Guid id, EndpointHandlerContext context)
    {
        var (db, hc, lg) = (context.DbContext, context.HttpContext, context.LinkGenerator);
        var author = db.Authors
            .AsNoTracking()
            .FirstOrDefault(a => a.Id == id);

        return author is null
            ? NotFound(author)
            : Ok(author
                .ToGetAuthor()
                .WithLinks(new
                {
                    self = lg.GetUriByName(hc, "GetAuthor", new { id }),
                    books = lg.GetUriByName(hc, "GetAuthorBooks", new { id }),
                }));
    }

    public static IResult GetAuthorBooks(Guid id, EndpointHandlerContext context)
    {
        var (db, hc, lg) = (context.DbContext, context.HttpContext, context.LinkGenerator);

        var author = db.Authors
            .AsNoTracking()
            .Include(a => a.Books)
            .FirstOrDefault(b => b.Id == id);

        return author is null
            ? NotFound(author)
            : Ok(author.Books
                .Select(b => b
                    .ToGetBook()
                    .WithLinks(new
                    {
                        self = lg.GetUriByName(hc, "GetBook", new { b.Id }),
                        authors = lg.GetUriByName(hc, "GetBookAuthors", new { b.Id }),
                    })));
    }


    public static IResult PostAuthor([FromBody]PostAuthor postAuthor, EndpointHandlerContext context)
    {
        var (db, hc, lg) = (context.DbContext, context.HttpContext, context.LinkGenerator);

        var validated = postAuthor.SimpleValidate();
        if (validated.IsValid is false) return BadRequest(new { validated.Errors });

        var author = db.Authors.Add(postAuthor.ToAuthor()).Entity;
        db.SaveChanges();

        var createdAt = lg.GetUriByName(hc, "GetAuthor", new { author.Id });
        return Created(createdAt, author.ToGetAuthor()
            .WithLinks(new
            {
                self = createdAt,
                books = lg.GetUriByName(hc, "GetAuthorBooks", new { author.Id }),
            }));
    }

    public static IResult PutAuthor(Guid id, [FromBody]PutAuthor putAuthor, EndpointHandlerContext context)
    {
        var (db, hc, lg) = (context.DbContext, context.HttpContext, context.LinkGenerator);

        var validated = putAuthor.SimpleValidate();
        if (validated.IsValid is false) return BadRequest(new { validated.Errors });

        var author = db.Authors.FirstOrDefault(b => b.Id == id);
        if (author is null) 
        {
            author = db.Authors.Add(putAuthor.ToAuthor()).Entity;
            db.SaveChanges();
            var createdAt = lg.GetUriByName(hc, "GetAuthor", new { author.Id });
            return Created(createdAt, author.ToGetAuthor()
                .WithLinks(new
                {
                    self = createdAt,
                    books = lg.GetUriByName(hc, "GetAuthorBooks", new { author.Id }),
                }));
        }
        else
        {
            author.Swap(putAuthor);
            db.SaveChanges();
            return Ok(author.ToGetAuthor()
                .WithLinks(new
                {
                    self = lg.GetUriByName(hc, "GetAuthor", new { author.Id }),
                    books = lg.GetUriByName(hc, "GetAuthorBooks", new { author.Id }),
                }));
        }
    }

    public static IResult PatchAuthor(Guid id, [FromBody]PatchAuthor patchAuthor, EndpointHandlerContext context)
    {
        var (db, hc, lg) = (context.DbContext, context.HttpContext, context.LinkGenerator);

        var validated = patchAuthor.SimpleValidate();
        if (validated.IsValid is false) return BadRequest(new { validated.Errors });

        var author = db.Authors.FirstOrDefault(b => b.Id == id);
        if (author is null) return NotFound();

        author.Update(patchAuthor);
        db.SaveChanges();

        return Ok(author.ToGetAuthor()
            .WithLinks(new
            {
                self = lg.GetUriByName(hc, "GetAuthor", new { author.Id }),
                books = lg.GetUriByName(hc, "GetAuthorBooks", new { author.Id }),
            }));
    }


    public static IResult DeleteAuthor(Guid id, EndpointHandlerContext context)
    {
        var (db, hc, lg) = (context.DbContext, context.HttpContext, context.LinkGenerator);

        var author = db.Authors.FirstOrDefault(b => b.Id == id);

        if (author is null) return NotFound(author);

        var res = db.Authors.Remove(author);
        db.SaveChanges();

        return Ok(new { id });
    }

    #region Hidden
    // public static IResult PostAuthorBook(Guid id, [FromBody]PostBook postBook, EndpointHandlerContext context)
    // {
    //     var (db, hc, lg) = (context.DbContext, context.HttpContext, context.LinkGenerator);

    //     var validated = postBook.SimpleValidate();
    //     if (validated.IsValid is false) return BadRequest(new { validated.Errors });

    //     var author = db.Authors.FirstOrDefault(a => a.Id == id);
    //     if (author is null) return NotFound();

    //     var book = db.Books.Add(postBook.ToBook()).Entity;
    //     author.AddBooks(book);
    //     db.SaveChanges();

    //     var createdAt = lg.GetUriByName(hc, "GetBook", new { book.Id });
    //     return Created(createdAt, book
    //         .ToGetBook()
    //         .WithLinks(new
    //         {
    //             self = createdAt,
    //             books = lg.GetUriByName(hc, "GetAuthorBooks", new { id }),
    //         }));
    // }
    #endregion
}
