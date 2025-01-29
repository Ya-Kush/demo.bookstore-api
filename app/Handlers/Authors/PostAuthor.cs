using App.Common.Extensions;
using App.Data;
using App.Data.Models;
using App.Handlers.Authors.Models;
using App.Handlers.Books;
using App.Handlers.HypermediaPrimitives;
using App.Handlers.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace App.Handlers.Authors;

public sealed class PostAuthor : IPostHandler
{
    public Delegate Handler => Handle;

    public readonly record struct PostAuthorRequest(string FirstName, string MiddleName, string LastName);

    async Task<Results<CreatedAtRoute<Item<PlainAuthor>>, BadRequest>> Handle([FromBody]PostAuthorRequest req, BookstoreDbContext db, EndpointContext context, CancellationToken cancel)
    {
        var errors = RequestValidator(req);
        if (errors.Count is not 0) return BadRequest();

        var authorRes = Author.New(req.FirstName, req.MiddleName, req.LastName);
        var author = authorRes.Value;

        await db.Authors.AddAsync(author, cancel);
        await db.SaveChangesAsync(cancel);

        return CreatedAtRoute(
            value: Item.New(
                links: author.GetLinks(context),
                acts: author.GetActs(context),
                props: author.ToPlain()),
            routeName: nameof(GetBook),
            routeValues: new { bookId = author.Id });
    }

    static List<string> RequestValidator(PostAuthorRequest req)
    {
        var errors = new List<string>();
        if (req.FirstName.IsNullOrWhiteSpace()) errors.Add("Wrong FirstName");
        if (req.MiddleName.IsNullOrWhiteSpace()) errors.Add("Wrong MiddleName");
        if (req.LastName.IsNullOrWhiteSpace()) errors.Add("Wrong LastName");
        return errors;
    }
}