using App.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace App.Handlers.Authors;

public sealed class DeleteAuthorBook : IDeleteHandler
{
    public Delegate Handler => Handle;

    async Task<Results<Ok,NotFound,BadRequest>> Handle([FromRoute]Guid authorId, [FromRoute]Guid bookId, BookstoreDbContext db, CancellationToken cancel)
    {
        var author = await db.Authors.Include(b => b.Books).FirstOrDefaultAsync(b => b.Id == bookId, cancel);
        if (author is null) return NotFound();
        var book = author.Books.FirstOrDefault(a => a.Id == bookId);
        if (book is null) return BadRequest();

        author.RemoveBook(book);

        await db.SaveChangesAsync(cancel);
        return Ok();
    }
}