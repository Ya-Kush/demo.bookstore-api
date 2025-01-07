using App.Data;
using App.Data.Models;
using App.Endpoints.Services;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace App.Endpoints;

public sealed class PostPublisher : IPostEndpoint
{
    readonly record struct PostPublisherRequest(string Name);

    public Delegate Handler => HandleAsync;

    async Task<IResult> HandleAsync([FromBody]PostPublisherRequest req, BookstoreDbContext db, EndpointContext context)
    {
        var pubRes = Publisher.New(req.Name);

        if (pubRes.IsFail) return BadRequest();

        var pub = pubRes.Value;
        await db.Publishers.AddAsync(pub);
        await db.SaveChangesAsync();

        return CreatedAtRoute(
            routeName: nameof(GetPublisher),
            routeValues: new { publisherId = pub.Id });
    }
}
