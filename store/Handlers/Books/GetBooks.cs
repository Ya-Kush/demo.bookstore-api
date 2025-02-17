using Store.Data;
using Store.Data.Models;
using Store.Handlers.Books.Models;
using Store.Handlers.HypermediaPrimitives;
using Store.Handlers.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace Store.Handlers.Books;

public sealed class GetBooks : IGetHandler
{
    public Delegate Handler => Handle;

    async Task<Ok<Set<PlainBook>>> Handle(BookstoreDbContext db, EndpointContext context, CancellationToken cancel)
    {
        var books = await db.Books.AsNoTracking().ToArrayAsync(cancel);
        Field[] addNewFields = [
            new("title", "string"),
            new("edition", "string"),
            new("price", "double"),
            new("publisherId", "guid?")];

        return Ok(books.ToSet(
            converter: b => Converter(b, context),
            links: [new("self", context.GetLinkFor<GetBooks>())],
            acts: [new("add_new", Act.Methods.POST, context.GetLinkFor<PostBook>(), addNewFields)]));
    }

    static Item<PlainBook> Converter(Book b, EndpointContext context)
        => Item.New(
            links: b.GetLinks(context),
            acts: b.GetActs(context),
            props: b.ToPlain()
        );
}