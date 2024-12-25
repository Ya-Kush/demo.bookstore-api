using App.Data;
using App.Endpoints.Models;
using App.Endpoints.Services;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace App.Endpoints;

public static class GetPublishers
{
    public readonly record struct Response(IEnumerable<SimplePublisherResponse> Data);

    public static IResult Handler(BookstoreDbContext db, EndpointContext context)
    {
        return Ok(new Response(db.Publishers.ToSimplePublisherResponses(context)));
    }
}
