using App.Data;
using App.Data.Models;
using App.Handlers.HypermediaPrimitives;
using App.Handlers.Publishers.Models;
using App.Handlers.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace App.Handlers.Publishers;

public sealed class PostPublisher : IPostHandler
{
    public Delegate Handler => HandleAsync;

    public readonly record struct PostPublisherRequest(string Name);

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