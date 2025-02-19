using Store.Data;
using Store.Data.Models;
using Store.Handlers.Books.Models;
using Store.Handlers.HypermediaPrimitives;
using Store.Handlers.Publishers.Models;
using Store.Handlers.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace Store.Handlers.Publishers;

public sealed class GetPublisherBooks : IGetHandler
{
    public Delegate Handler => Handle;

    async Task<Results<Ok<Set<PlainPublisherBook>>,NotFound>> Handle(Guid publisherId, BookstoreDbContext db, EndpointContext context, CancellationToken cancel)
    {
        var pub = await db.Publishers.AsNoTracking().Include(p => p.Books).FirstOrDefaultAsync(p => p.Id == publisherId, cancel);
        if (pub is null) return NotFound();

        object publisherIdValues = new { publisherId };
        Act[] acts = [new(
            Name: "add_book",
            Method: Act.Methods.POST,
            Href: context.GetLinkFor<PostPublisherBook>(publisherIdValues),
            Fields: [new("bookId", "guid")])];

        return Ok(pub.Books.ToSet(
                converter: b => Converter(b, context, pub),
                links: [new("self", context.GetLinkFor<GetPublisherBooks>(publisherIdValues))],
                acts: acts
        ));
    }

    static Item<PlainPublisherBook> Converter(Book b, EndpointContext context, Publisher pub)
        => Item.New(
            links: b.GetLinks(context),
            acts: b.GetPublishersActs(pub, context),
            props: b.ToPublisherPlain()
        );
}