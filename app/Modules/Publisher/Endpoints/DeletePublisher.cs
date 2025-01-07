using App.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace App.Endpoints;

public sealed class DeletePublisher : IDeleteEndpoint
{
    public Delegate Handler => Handle;

    async Task<Results<Ok,NotFound>> Handle(Guid publisherId, BookstoreDbContext db)
    {
        var deletedCount = await db.Publishers.Where(p => p.Id == publisherId).ExecuteDeleteAsync();
        return deletedCount == 0 ? NotFound() : Ok();
    }
}
