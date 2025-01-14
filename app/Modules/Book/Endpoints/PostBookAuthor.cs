using App.Data;
using App.Endpoints.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace App.Endpoints;

public sealed class PostBookAuthor : IPostEndpoint
{
    public Delegate Handler => Handle;

    async Task<Results<Ok,NotFound,BadRequest>> Handle([FromRoute]Guid bookId, [FromBody]Guid authorId, BookstoreDbContext db, EndpointContext context, CancellationToken cancel)
    {
        var book = await db.Books.FirstOrDefaultAsync(b => b.Id == bookId, cancel);
        var author = await db.Authors.FirstOrDefaultAsync(a => a.Id == authorId, cancel);

        if (book is null) return NotFound();
        if (author is null || book.HasEachOther(author)) return BadRequest();

        book.AddAuthor(author);
        await db.SaveChangesAsync(cancel);

        return Ok();
    }
}