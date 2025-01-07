using App.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace App.Endpoints;

public sealed class DeletePublisherBook : IDeleteEndpoint
{
    public Delegate Handler => Handle;

    async Task<Results<Ok,NoContent,NotFound>> Handle(Guid publisherId, Guid bookId, BookstoreDbContext db)
    {
        var pub = await db.Publishers.Include(p => p.Books).FirstOrDefaultAsync(p => p.Id == publisherId);
        var book = await db.Books.FirstOrDefaultAsync(b => b.Id == bookId);

        if (pub is null) return NotFound();
        if (book is null) return NotFound();
        if (pub.HasEachOther(book) is false) return NoContent();

        pub.RemoveBook(book);
        await db.SaveChangesAsync();

        return Ok();
    }
}