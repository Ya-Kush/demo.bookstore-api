using App.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace App.Endpoints;

public sealed class DeleteAuthor : IDeleteEndpoint
{
    public Delegate Handler => Handle;

    async Task<Results<Ok,NotFound>> Handle([FromRoute]Guid authorId, BookstoreDbContext db, CancellationToken cancel)
        => await db.Authors.Where(a => a.Id == authorId).ExecuteDeleteAsync(cancel) is 0 ? NotFound() : Ok();
}
