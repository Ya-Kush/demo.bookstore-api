using App.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace App.Endpoints;

public sealed class DeletePublisherBook : IDeleteEndpoint
{
    public Delegate Handler => Handle;

    async Task<Results<Ok,NotFound,BadRequest>> Handle([FromRoute]Guid publisherId, [FromRoute]Guid bookId, BookstoreDbContext db, CancellationToken cancel)
    {
        var pub = await db.Publishers.Include(p => p.Books).FirstOrDefaultAsync(p => p.Id == publisherId, cancel);
        var book = await db.Books.FirstOrDefaultAsync(b => b.Id == bookId, cancel);

        if (pub is null) return NotFound();
        if (book is null) return NotFound();
        if (pub.HasEachOther(book) is false) return BadRequest();

        pub.RemoveBook(book);
        await db.SaveChangesAsync(cancel);

        return Ok();
    }
}