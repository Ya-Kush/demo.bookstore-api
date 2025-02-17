using Store.Data;
using Store.Handlers.Authors.Models;
using Store.Handlers.HypermediaPrimitives;
using Store.Handlers.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace Store.Handlers.Authors;

public sealed class GetAuthor : IGetHandler
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