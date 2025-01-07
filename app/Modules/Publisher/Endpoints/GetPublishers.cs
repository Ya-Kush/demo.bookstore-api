using App.Data;
using App.Data.Extensions;
using App.Endpoints.Models;
using App.Endpoints.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace App.Endpoints;

public sealed class GetPublishers : IGetEndpoint
{
    readonly record struct GetPublishersItem(FlatPublisher Publisher, Link[] Links, Act[] Acts);
    readonly record struct GetPublishersResponse(IEnumerable<GetPublishersItem> Data, Act[] Acts);

    public Delegate Handler => Handle;

    async Task<Ok<GetPublishersResponse>> Handle(BookstoreDbContext db, EndpointContext context)
    {
        var data = await db.Publishers.Untrack().ToArrayAsync();

        return Ok(new GetPublishersResponse(
            Data: data.Select(pub => new GetPublishersItem
            {
                Publisher = pub.ToFlat(),
                Links = pub.GetLinks(context),
                Acts = pub.GetActs(context)
            }),
            Acts:
            [
                new(Rel: "add_new",
                    Method: Act.Methods.POST,
                    Href: context.GetLinkBy<PostPublisher>()),
            ]));
    }
}
