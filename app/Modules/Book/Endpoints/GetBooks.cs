using App.Data;
using App.Data.Extensions;
using App.Data.Models;
using App.Endpoints.HypermediaPrimitives;
using App.Endpoints.Models;
using App.Endpoints.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace App.Endpoints;

public sealed class GetBooks : IGetEndpoint
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
