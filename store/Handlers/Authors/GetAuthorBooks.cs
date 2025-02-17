using Store.Data;
using Store.Data.Models;
using Store.Handlers.Authors.Models;
using Store.Handlers.Books;
using Store.Handlers.Books.Models;
using Store.Handlers.HypermediaPrimitives;
using Store.Handlers.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace Store.Handlers.Authors;

public sealed class GetAuthorBooks : IGetHandler
{
    public Delegate Handler => Handle;

    async Task<Results<Ok<Set<PlainAuthorBook>>,NotFound>> Handle([FromRoute]Guid authorId, BookstoreDbContext db, EndpointContext context, CancellationToken cancel)
    {
        var author = await db.Authors.AsNoTracking().FirstOrDefaultAsync(a => a.Id == authorId, cancel);
        if (author is null) return NotFound();

        object authorIdValues = new { authorId };
        Act[] acts = [new(
            Name: "add_book",
            Method: Act.Methods.POST,
            Href: context.GetLinkFor<PostAuthorBook>(authorIdValues),
            Fields: [new("bookId", "guid")])];

        return Ok(author.Books.ToSet(
            converter: book => Converter(book, context, author),
            links: [new("self", context.GetLinkFor<GetBookAuthors>(authorIdValues))],
            acts: acts
        ));
    }

    static Item<PlainAuthorBook> Converter(Book book, EndpointContext context, Author author)
        => Item.New(
            links: book.GetLinks(context),
            acts: book.GetAuthorActs(author, context),
            props: book.ToAuthorPlain()
        );
}