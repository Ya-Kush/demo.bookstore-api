using App.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace App.Endpoints;

public sealed class PatchPublisher : IPatchEndpoint
{
    readonly record struct PatchPublisherRequest(string? Name);

    public Delegate Handler => Handle;

    async Task<Results<Ok,BadRequest>> Handle(Guid publisherId, PatchPublisherRequest req, BookstoreDbContext db)
    {
        var pub = await db.Publishers.FirstOrDefaultAsync(p => p.Id == publisherId);
        if (pub is null) return BadRequest();

        if (req.Name is not null) pub.Name = req.Name;
        await db.SaveChangesAsync();

        return Ok();
    }
}
