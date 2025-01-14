using App.Data;
using App.Data.Models;
using App.Endpoints.HypermediaPrimitives;
using App.Endpoints.Models;
using App.Endpoints.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace App.Endpoints;

public sealed class PostPublisher : IPostEndpoint
{
    readonly record struct PostPublisherRequest(string Name);

    public Delegate Handler => HandleAsync;

    async Task<Results<CreatedAtRoute<Item<PlainPublisher>>,BadRequest>> HandleAsync([FromBody]PostPublisherRequest req,
            BookstoreDbContext db, EndpointContext context, CancellationToken cancel)
    {
        var pubRes = Publisher.New(req.Name);

        if (pubRes.IsFail) return BadRequest();

        var pub = pubRes.Value;
        await db.Publishers.AddAsync(pub, cancel);
        await db.SaveChangesAsync(cancel);

        return CreatedAtRoute(
            value: Item.New(pub.GetLinks(context), pub.GetActs(context), pub.ToPlain()),
            routeName: nameof(GetPublisher),
            routeValues: new { publisherId = pub.Id });
    }
}
