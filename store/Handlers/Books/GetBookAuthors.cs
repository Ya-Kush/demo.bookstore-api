using Store.Data;
using Store.Data.Models;
using Store.Handlers.Authors.Models;
using Store.Handlers.Books.Models;
using Store.Handlers.HypermediaPrimitives;
using Store.Handlers.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace Store.Handlers.Books;

public sealed class GetBookAuthors : IGetHandler
{
    public Delegate Handler => Handle;

    async Task<Results<Ok<Set<PlainBookAuthor>>,NotFound>> Handle([FromRoute]Guid bookId, BookstoreDbContext db, EndpointContext context, CancellationToken cancel)
    {
        var book = await db.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == bookId, cancel);
        if (book is null) return NotFound();

        object bookIdValues = new { bookId };
        Act[] acts = [new("add_author", Act.Methods.POST, context.GetLinkFor<PostBookAuthor>(bookIdValues), [new("authorId", "guid")])];

        return Ok(book.Authors.ToSet(
            converter: author => Converter(author, context, book),
            links: [new("self", context.GetLinkFor<GetBookAuthors>(bookIdValues))],
            acts: acts
        ));
    }

    static Item<PlainBookAuthor> Converter(Author author, EndpointContext context, Book book)
        => Item.New(
            links: author.GetLinks(context),
            acts: author.GetBookActs(book, context),
            props: author.ToBookPlain()
        );
}