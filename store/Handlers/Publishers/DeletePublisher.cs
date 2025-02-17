using Store.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace Store.Handlers.Publishers;

public sealed class DeletePublisher : IDeleteHandler
{
    public Delegate Handler => Handle;

    async Task<Results<Ok,NotFound>> Handle([FromRoute]Guid publisherId, BookstoreDbContext db, CancellationToken cancel)
        => await db.Publishers.Where(p => p.Id == publisherId).ExecuteDeleteAsync(cancel) is 0 ? NotFound() : Ok();
}