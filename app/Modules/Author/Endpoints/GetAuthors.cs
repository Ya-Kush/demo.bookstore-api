using App.Data;
using App.Data.Models;
using App.Endpoints.HypermediaPrimitives;
using App.Endpoints.Models;
using App.Endpoints.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace App.Endpoints;

public sealed class GetAuthors : IGetEndpoint
{
    public Delegate Handler => Handle;

    async Task<Ok<Set<PlainAuthor>>> Handle(BookstoreDbContext db, EndpointContext context, CancellationToken cancel)
    {
        var author = await db.Authors.AsNoTracking().ToArrayAsync(cancel);
        Field[] addNewFields = [
            new("firstName", "string"),
            new("middlerName", "string"),
            new("lastName", "string")];

        return Ok(author.ToSet(
            converter: author => Converter(author, context),
            links: [new("self", context.GetLinkFor<GetAuthors>())],
            acts: [new("add_new", Act.Methods.POST, context.GetLinkFor<PostAuthor>(), addNewFields)]));
    }

    static Item<PlainAuthor> Converter(Author author, EndpointContext context)
        => Item.New(
            links: author.GetLinks(context),
            acts: author.GetActs(context),
            props: author.ToPlain()
        );
}