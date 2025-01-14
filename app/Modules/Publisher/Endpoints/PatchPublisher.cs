using App.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace App.Endpoints;

public sealed class PatchPublisher : IPatchEndpoint
{
    readonly record struct PatchPublisherRequest(string? Name);

    public Delegate Handler => Handle;

    async Task<Results<Ok,BadRequest>> Handle(Guid publisherId, PatchPublisherRequest req, BookstoreDbContext db, CancellationToken cancel)
    {
        var pub = await db.Publishers.FirstOrDefaultAsync(p => p.Id == publisherId, cancel);
        if (pub is null) return BadRequest();

        if (req.Name is not null)
            if (req.Name is "") return BadRequest();
            else pub.Name = req.Name;
        await db.SaveChangesAsync(cancel);

        return Ok();
    }
}
