using Store.Data;
using Store.Data.Models;
using Store.Handlers.HypermediaPrimitives;
using Store.Handlers.Publishers.Models;
using Store.Handlers.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace Store.Handlers.Publishers;

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