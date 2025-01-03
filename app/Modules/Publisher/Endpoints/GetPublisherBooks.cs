using App.Data;
using App.Data.Extensions;
using App.Endpoints.Models;
using App.Endpoints.Services;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace App.Endpoints;

public static class GetPublisherBooks
{
    public readonly record struct Response(IEnumerable<GetBook> Data);

    public static IResult Handle(Guid publisherId, BookstoreDbContext db, EndpointContext context)
    {
        var pub = db.Publishers.Untrack().Include(p => p.Books).FirstOrDefault(p => p.Id == publisherId);
        return Ok(new Response(pub?.Books.ToGetBooks(context) ?? []));
    }
}
