using App.Data;
using App.Data.Extensions;
using App.Endpoints.Models;
using App.Endpoints.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace App.Endpoints;

public sealed class GetPublisher : IGetEndpoint
{
    readonly record struct GetPublisherResponse(FlatPublisher Data, Link[] Links, Act[] Acts);

    public Delegate Handler => Handle;

    async Task<Results<Ok<GetPublisherResponse>,NotFound>> Handle(Guid publisherId, BookstoreDbContext db, EndpointContext context)
    {
        var pub = await db.Publishers.Untrack().FirstOrDefaultAsync(p => p.Id == publisherId);

        return pub is null ? NotFound()
            : Ok(new GetPublisherResponse
            {
                Data = pub.ToFlat(),
                Links = pub.GetLinks(context),
                Acts = pub.GetActs(context)
            });
    }
}
