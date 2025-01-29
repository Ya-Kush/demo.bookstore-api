using App.Common.Extensions;
using App.Data;
using App.Data.Models;
using App.Handlers.Books.Models;
using App.Handlers.HypermediaPrimitives;
using App.Handlers.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace App.Handlers.Books;

public sealed class PostBook : IPostHandler
{
    public Delegate Handler => Handle;

    public readonly record struct PostBookRequest(string Title, string Edition, Guid? PublisherId, double Price);

    async Task<Results<CreatedAtRoute<Item<PlainBook>>, BadRequest>> Handle([FromBody]PostBookRequest req, BookstoreDbContext db, EndpointContext context, CancellationToken cancel)
    {
        var errors = RequestValidator(req);
        if (errors.Count is not 0) return BadRequest();

        Publisher? publisher = null;
        if (req.PublisherId is not null)
        {
            publisher = await db.Publishers.FirstOrDefaultAsync(p => p.Id == req.PublisherId, cancel);
            if (publisher is null) return BadRequest();
        }

        var bookRes = Book.New(req.Title, req.Edition, req.Price, publisher);
        var book = bookRes.Value;

        await db.Books.AddAsync(book, cancel);
        await db.SaveChangesAsync(cancel);

        return CreatedAtRoute(
            value: Item.New(
                links: book.GetLinks(context),
                acts: book.GetActs(context),
                props: book.ToPlain()),
            routeName: nameof(GetBook),
            routeValues: new { bookId = book.Id });
    }

    static List<string> RequestValidator(PostBookRequest req)
    {
        var errors = new List<string>();
        if (req.Title.IsNullOrWhiteSpace()) errors.Add("Wrong Title");
        if (req.Edition.IsNullOrWhiteSpace()) errors.Add("Wrong Edition");
        if (req.Price <= 0) errors.Add("Wrong Price");
        return errors;
    }
}