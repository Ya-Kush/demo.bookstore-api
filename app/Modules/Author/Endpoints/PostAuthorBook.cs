using App.Data;
using App.Endpoints.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace App.Endpoints;

public sealed class PostAuthorBook : IPostEndpoint
{
    public Delegate Handler => Handle;

    async Task<Results<Ok,NotFound,BadRequest>> Handle([FromRoute]Guid authorId, [FromBody]Guid bookId, BookstoreDbContext db, EndpointContext context, CancellationToken cancel)
    {
        var author = await db.Authors.FirstOrDefaultAsync(a => a.Id == authorId, cancel);
        if (author is null) return NotFound();
        var book = await db.Books.FirstOrDefaultAsync(b => b.Id == bookId, cancel);
        if (book is null) return BadRequest();

        author.AddBook(book);
        await db.SaveChangesAsync(cancel);

        return Ok();
    }
}