using App.Data;
using App.Data.Extensions;
using App.Endpoints.Models;
using App.Endpoints.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace App.Endpoints;

public sealed class GetPublisherBooks : IGetEndpoint
{
    readonly record struct GetPublisherBooksItem(FlatPublisherBook Book, Link[] Links, Act[] Acts);
    readonly record struct GetPublisherBooksResponse(IEnumerable<GetPublisherBooksItem> Data);

    public Delegate Handler => Handle;

    async Task<Results<Ok<GetPublisherBooksResponse>,NotFound>> Handle(Guid publisherId, BookstoreDbContext db, EndpointContext context)
    {
        var pub = await db.Publishers.Untrack().Include(p => p.Books).FirstOrDefaultAsync(p => p.Id == publisherId);

        return pub is null ? NotFound()
            : Ok(new GetPublisherBooksResponse(pub.Books.Select(b => new GetPublisherBooksItem
            {
                Book = new(b.Id, b.Title, b.Edition, b.Price),
                Links = b.GetLinks(context),
                Acts = b.GetPublishersActs(pub, context)
            })));
    }
}
