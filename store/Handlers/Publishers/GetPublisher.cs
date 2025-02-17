using Store.Data;
using Store.Handlers.HypermediaPrimitives;
using Store.Handlers.Publishers.Models;
using Store.Handlers.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace Store.Handlers.Publishers;

public sealed class GetPublisher : IGetHandler
{
    public Delegate Handler => Handle;

    async Task<Results<Ok<Item<PlainPublisher>>,NotFound>> Handle(Guid publisherId, BookstoreDbContext db, EndpointContext context, CancellationToken cancel)
    {
        var pub = await db.Publishers.AsNoTracking().FirstOrDefaultAsync(p => p.Id == publisherId, cancel);

        return pub is null
            ? NotFound()
            : Ok(Item.New(
                links: pub.GetLinks(context),
                acts: pub.GetActs(context),
                props: pub.ToPlain()));
    }
}