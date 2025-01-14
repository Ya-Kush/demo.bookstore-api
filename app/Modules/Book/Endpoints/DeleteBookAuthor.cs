using App.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace App.Endpoints;

public sealed class DeleteBookAuthor : IDeleteEndpoint
{
    public Delegate Handler => Handle;

    async Task<Results<Ok,NotFound>> Handle([FromRoute]Guid bookId, [FromRoute]Guid authorId, BookstoreDbContext db, CancellationToken cancel)
    {
        var book = await db.Books.Include(b => b.Authors).FirstOrDefaultAsync(b => b.Id == bookId, cancel);
        var author = book?.Authors.FirstOrDefault(a => a.Id == authorId);
        if (book is null || author is null) return NotFound();

        book.RemoveAuthor(author);

        await db.SaveChangesAsync(cancel);
        return Ok();
    }
}