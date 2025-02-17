using Store.Data;
using Store.Data.Models;
using Store.Handlers.HypermediaPrimitives;
using Store.Handlers.Publishers.Models;
using Store.Handlers.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace Store.Handlers.Publishers;

public sealed class GetPublishers : IGetHandler
{
    public Delegate Handler => Handle;

    async Task<Ok<Set<PlainPublisher>>> Handle(BookstoreDbContext db, EndpointContext context, CancellationToken cancel)
    {
        var pubs = await db.Publishers.AsNoTracking().ToArrayAsync(cancel);

        Act[] acts = [new(
            Name: "add_new",
            Method: Act.Methods.POST,
            Href: context.GetLinkFor<PostPublisher>(),
            Fields: [new("name", "string")])];

        return Ok(pubs.ToSet(
            converter: pub => Converter(pub, context),
            links: [],
            acts: acts
        ));
    }

    static Item<PlainPublisher> Converter(Publisher pub, EndpointContext context)
        => Item.New(
            links: pub.GetLinks(context),
            acts: pub.GetActs(context),
            props: pub.ToPlain()
        );
}