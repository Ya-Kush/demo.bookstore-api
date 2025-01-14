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

public sealed class GetBook : IGetEndpoint
{
    public Delegate Handler => Handle;

    async Task<Results<Ok<Item<PlainBook>>,NotFound>> Handle([FromRoute]Guid bookId, BookstoreDbContext db, EndpointContext context, CancellationToken cancel)
    {
        var book = await db.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == bookId, cancel);

        return book is null
            ? NotFound()
            : Ok(Item.New(
                links: book.GetLinks(context),
                acts: book.GetActs(context),
                props: book.ToPlain()
            ));
    }
}
