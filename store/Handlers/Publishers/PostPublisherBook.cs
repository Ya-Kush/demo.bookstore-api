using Store.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace Store.Handlers.Publishers;

public sealed class PostPublisherBook : IPostHandler
{
    public Delegate Handler => Handle;

    async Task<Results<Ok,NotFound,BadRequest>> Handle([FromRoute]Guid publisherId, [FromBody]Guid bookId, BookstoreDbContext db, CancellationToken cancel)
    {
        var pub = await db.Publishers.Include(p => p.Books).FirstOrDefaultAsync(p => p.Id == publisherId, cancel);
        var book = await db.Books.FirstOrDefaultAsync(b => b.Id == bookId, cancel);

        if (pub is null) return NotFound();
        if (book is null) return NotFound();
        if (pub.HasEachOther(book)) return BadRequest();

        pub.AddBook(book);
        await db.SaveChangesAsync(cancel);

        return Ok();
    }
}