using App.Common;
using App.Data;
using App.Data.Extensions;
using App.Data.Models;
using App.Endpoints.Models;
using App.Endpoints.Models.Validators;
using App.Services.Errors;
using Microsoft.EntityFrameworkCore;

namespace App.Endpoints.Services;

public sealed class AuthorRepo(BookstoreDbContext bookstoreDbContext)
{
    BookstoreDbContext DbContext { get; } = bookstoreDbContext;

    #region Access
    public IQueryable<Author> Authors => DbContext.Authors;

    public Res<Author> FindAuthor(Guid authorId) => Authors.FirstOrDefault(a => a.Id == authorId);
    public Res<Author> FindAuthorWithBooks(Guid authorId) => Authors.WithBooks().FirstOrDefault(a => a.Id == authorId);
    public Res<IEnumerable<Book>> FindAuthorBooks(Guid authorId) => FindAuthorWithBooks(authorId).Map(a => a.Books.ToRes());

    public Res<Author> GetAuthor(Guid authorId) => Authors.Untrack().FirstOrDefault(a => a.Id == authorId);
    public Res<Author> GetAuthorWithBooks(Guid authorId) => Authors.Untrack().WithBooks().FirstOrDefault(a => a.Id == authorId);
    public Res<IEnumerable<Book>> GetAuthorBooks(Guid authorId) => GetAuthorWithBooks(authorId).Map(a => a.Books.ToRes());
    #endregion Access

    public Res<Author> SaveNewAuthor(PostAuthor postAuthor)
    {
        var val = postAuthor.SimpleValidate();
        return val.IsValid
            ? SaveAuthor(postAuthor.ToAuthor())
            : val.Error;
    }

    public Res<Author> UpdateAuthor(Guid authorId, PatchAuthor patchAuthor)
    {
        var authorRes = FindAuthor(authorId);
        var valRes = patchAuthor.SimpleValidate();

        if (authorRes.IsFail && valRes.IsInvalid) return authorRes.Error.Concat(valRes.Error);
        else if (authorRes.IsFail) return authorRes.Error;
        else if (valRes.IsInvalid) return valRes.Error;
        else return authorRes.Match(
            book => UpdateAuthor(book, patchAuthor),
            err => err);
    }

    public Res DeleteAuthor(Guid authorId)
    {
        var authorRes = FindAuthor(authorId);
        return authorRes.Match(
            author =>
            {
                DbContext.Authors.Remove(author);
                DbContext.SaveChanges();
                return Res.Ok();
            },
            err => err);
    }

    public Res AddBookToAuthor(Guid authorId, Guid bookId)
    {
        var authorRes = FindAuthorWithBooks(authorId);
        var bookRes = DbContext.Books
            .Include(b => b.Authors)
            .FirstOrDefault(b => b.Id == bookId)
            .ToRes(ifNull: () => new NotFoundError());

        if (authorRes.IsFail && bookRes.IsFail) return authorRes.Error.Concat(bookRes.Error);
        else if (authorRes.IsFail && bookRes.IsOk) return authorRes.Error;
        else if (authorRes.IsOk && bookRes.IsFail) return bookRes.Error;
        authorRes.Value.AddBook(bookRes.Value);
        DbContext.SaveChanges();
        return Res.Ok();
    }

    public Res RemoveBookFromAuthor(Guid authorId, Guid bookId)
    {
        var authorRes = FindAuthorWithBooks(authorId);
        var bookRes = DbContext.Books
            .Include(b => b.Authors)
            .FirstOrDefault(b => b.Id == bookId)
            .ToRes(ifNull: () => new NotFoundError());

        if (authorRes.IsFail && bookRes.IsFail) return authorRes.Error.Concat(bookRes.Error);
        else if (authorRes.IsFail && bookRes.IsOk) return authorRes.Error;
        else if (authorRes.IsOk && bookRes.IsFail) return bookRes.Error;
        authorRes.Value.RemoveBook(bookRes.Value);
        DbContext.SaveChanges();
        return Res.Ok();
    }


    Res<Author> SaveAuthor(Author author)
    {
        var entry = DbContext.Authors.Add(author);
        DbContext.SaveChanges();
        return entry.Entity;
    }

    Res<Author> UpdateAuthor(Author author, PatchAuthor patchAuthor)
    {
        author.Update(patchAuthor);
        DbContext.SaveChanges();
        return author;
    }
}
