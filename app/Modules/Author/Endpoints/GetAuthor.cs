using App.Data;
using App.Data.Extensions;
using App.Endpoints.HypermediaPrimitives;
using App.Endpoints.Models;
using App.Endpoints.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace App.Endpoints;

public sealed class GetAuthor : IGetEndpoint
{
    public Delegate Handler => Handle;

    async Task<Results<Ok<Item<PlainAuthor>>,NotFound>> Handle([FromRoute]Guid authorId, BookstoreDbContext db, EndpointContext context, CancellationToken cancel)
    {
        var author = await db.Authors.AsNoTracking().FirstOrDefaultAsync(a => a.Id == authorId, cancel);

        return author is null
            ? NotFound()
            : Ok(Item.New(
                links: author.GetLinks(context),
                acts: author.GetActs(context),
                props: author.ToPlain()
            ));
    }
}