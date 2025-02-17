using Store.Common.Extensions;
using Store.Data;
using Store.Data.Models;
using Store.Handlers.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace Store.Handlers.Authors;

public sealed class PatchAuthor : IPatchHandler
{
    public Delegate Handler => Handle;

    public readonly record struct PatchAuthorRequest(string FirstName, string MiddleName, string LastName);

    async Task<Results<Ok,NotFound,BadRequest>> Handle([FromRoute]Guid authorId, [FromBody]PatchAuthorRequest req, BookstoreDbContext db, EndpointContext context, CancellationToken cancel)
    {
        var author = await db.Authors.FirstOrDefaultAsync(a => a.Id == authorId, cancel);
        if (author is null) return NotFound();

        var errors = RequestValidator(req);
        if (errors.Count is not 0) return BadRequest();

        Swap(author, req);
        await db.SaveChangesAsync(cancel);

        return Ok();
    }

    static List<string> RequestValidator(PatchAuthorRequest req)
    {
        var errors = new List<string>();
        if (req.FirstName is not null && req.FirstName.IsNullOrWhiteSpace()) errors.Add("Wrong FirstName");
        if (req.MiddleName is not null && req.MiddleName.IsNullOrWhiteSpace()) errors.Add("Wrong MiddleName");
        if (req.LastName is not null && req.LastName.IsNullOrWhiteSpace()) errors.Add("Wrong LastName");
        return errors;
    }

    static void Swap(Author author, PatchAuthorRequest req)
    {
        if (req.FirstName is not null) author.FirstName = req.FirstName;
        if (req.MiddleName is not null) author.MiddleName = req.MiddleName;
        if (req.LastName is not null) author.LastName = req.LastName;
    }
}