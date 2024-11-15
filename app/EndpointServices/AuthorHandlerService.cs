using App.Common;
using App.Data;
using App.DomainModels;
using App.EndpointModels;
using App.EndpointModelValidators;
using Microsoft.EntityFrameworkCore;
using static App.Common.Res;

namespace App.EndpointServices;

public sealed class AuthorHandlerService(EndpointHandlerContext context, BookstoreDbContext bookstoreDbContext) : EndpointHandlerService(context)
{
    BookstoreDbContext DbContext { get; } = bookstoreDbContext;

    public IQueryable<Author> Authors => DbContext.Authors;
    public IQueryable<Author> UntrackedAuthors => DbContext.Authors.AsNoTracking();

    public IRes<Author> FindUntracked(Guid id) => GetUntrackedAuthor(id).IfNull(NotFound, Ok);
    public IRes<Author> Find(Guid id) => GetAuthor(id).IfNull(NotFound, Ok);
    public IRes<IEnumerable<Book>> FindAuthorBooks(Guid id)
        => UntrackedAuthors
            .Include(x => x.Books)
            .FirstOrDefault(x => x.Id == id)?.Books
                .IfNull(NotFound, Ok)!;

    Author? GetUntrackedAuthor(Guid id) => UntrackedAuthors.FirstOrDefault(x => x.Id == id);
    Author? GetAuthor(Guid id) => DbContext.Authors.FirstOrDefault(x => x.Id == id);

    public IRes<Author> SaveNewAuthor(PostAuthor postAuthor)
    {
        var val = postAuthor.SimpleValidate();
        return val.IsValid
            ? SaveAuthor(postAuthor.ToAuthor())
            : Error<Author>(val.Errors);
    }

    public IRes<Author> SaveAuthor(Guid id, PutAuthor putAuthor)
    {
        var author = GetAuthor(id);
        var val = putAuthor.SimpleValidate();
        if (val.IsInvalid) return Error<Author>(val.Errors);

        return author.IfNull(
            a => SaveNewAuthorWithId(id, putAuthor),
            a => ReplaceAuthor(a, putAuthor.ToAuthor()));
    }

    public IRes<Author> UpdateAuthor(Guid id, PatchAuthor patchAuthor)
    {
        var author = GetAuthor(id);
        var val = patchAuthor.SimpleValidate();
        var err = author is null ? ["Author Not Found", ..val.Errors] : val.Errors;

        if (err.Any()) return Error(author, err);

        return UpdateAuthor(author!, patchAuthor);
    }

    public IRes<Author> DeleteAuthor(Guid id)
    {
        var author = GetAuthor(id);
        if (author is null) return Fail(author);

        DbContext.Authors.Remove(author);
        DbContext.SaveChanges();

        return Ok(author);
    }


    IRes<Author> SaveNewAuthorWithId(Guid id, PutAuthor putAuthor)
    {
        var author = Author.NewWithId(id, putAuthor.FirstName, putAuthor.MiddleName, putAuthor.LastName, []);
        DbContext.Authors.Add(author);
        DbContext.SaveChanges();
        return Ok(author);
    }

    IRes<Author> SaveAuthor(Author author)
    {
        var entry = DbContext.Authors.Add(author);
        DbContext.SaveChanges();
        return Ok(entry.Entity);
    }

    IRes<Author> UpdateAuthor(Author author, PatchAuthor patchAuthor)
    {
        author.Update(patchAuthor);
        DbContext.SaveChanges();
        return Ok(author);
    }

    IRes<Author> ReplaceAuthor(Author author, Author newAuthor)
    {
        author.FirstName = newAuthor.FirstName;
        author.MiddleName = newAuthor.MiddleName;
        author.LastName = newAuthor.LastName;

        author.RemoveBooks(author.Books);
        author.AddBooks(newAuthor.Books);

        DbContext.SaveChanges();

        return Updated(author);
    }
}
