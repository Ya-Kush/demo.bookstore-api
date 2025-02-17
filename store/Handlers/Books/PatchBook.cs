using Store.Data;
using Store.Data.Models;
using Store.Handlers.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace Store.Handlers.Books;

public sealed class PatchBook : IPatchHandler
{
    public Delegate Handler => Handle;

    public readonly record struct PatchBookRequest(string? Title, string? Edition, Guid? PublisherId, double? Price);

    async Task<Results<Ok,NotFound,BadRequest>> Handle([FromRoute]Guid bookId, [FromBody]PatchBookRequest req, BookstoreDbContext db, EndpointContext context, CancellationToken cancel)
    {
        var book = await db.Books.FirstOrDefaultAsync(b => b.Id == bookId, cancel);
        if (book is null) return NotFound();

        var errors = RequestValidator(req);
        if (errors.Count is not 0) BadRequest();

        Publisher? publisher = null;
        if (req.PublisherId is not null)
        {
            publisher = await db.Publishers.FirstOrDefaultAsync(p => p.Id == req.PublisherId, cancel);
            if (publisher is null) return BadRequest();
        }

        Swap(book, req);
        await db.SaveChangesAsync(cancel);

        return Ok();
    }

    static List<string> RequestValidator(PatchBookRequest req)
    {
        var res = new List<string>();
        if (req.Title is not null and "") res.Add("Wrong Title");
        if (req.Edition is not null and "") res.Add("Wrong Edition");
        if (req.Price is not null and < 0) res.Add("The Price is less than zero");
        return res;
    }

    static void Swap(Book book, PatchBookRequest req)
    {
        if (req.Title is not null) book.Title = req.Title;
        if (req.Edition is not null) book.Edition = req.Edition;
        if (req.Price is not null) book.Price = req.Price.Value;
    }
}