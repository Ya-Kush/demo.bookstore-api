using App.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace App.Handlers.Books;

public sealed class DeleteBook : IDeleteHandler
{
    public Delegate Handler => Handle;

    async Task<Results<Ok,NotFound>> Handle([FromRoute]Guid bookId, BookstoreDbContext db, CancellationToken cancel)
        => await db.Books.Where(b => b.Id == bookId).ExecuteDeleteAsync(cancel) is 0 ? NotFound() : Ok();
}