using App.Data;
using App.Data.Extensions;
using App.Endpoints.Models;
using App.Endpoints.Services;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace App.Endpoints;

public static class GetPublisher
{
    public readonly record struct Response(SimplePublisherResponse? Data);

    public static IResult Handler(Guid publisherId, BookstoreDbContext db, EndpointContext context)
    {
        var pub = db.Publishers.Untrack().FirstOrDefault(p => p.Id == publisherId);
        return Ok(new Response(pub?.ToSimplePublisherResponse(context)));
    }
}
