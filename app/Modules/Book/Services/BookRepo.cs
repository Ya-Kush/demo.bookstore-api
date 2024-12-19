using App.Common;
using App.Data;
using App.Data.Extensions;
using App.Data.Models;
using App.Endpoints.Models;
using App.Endpoints.Models.Validators;
using App.Services.Errors;
using Microsoft.EntityFrameworkCore;

namespace App.Endpoints.Services;

public sealed class BookRepo(BookstoreDbContext bookstoreDbContext)
{
    BookstoreDbContext DbContext { get; } = bookstoreDbContext;

    #region Access
    public IQueryable<Book> Books => DbContext.Books;

    public Res<Book> FindBook(Guid bookId) => Books.FirstOrDefault(b => b.Id == bookId);
    public Res<Book> FindBookWithAuthors(Guid bookId) => Books.WithAuthors().FirstOrDefault(b => b.Id == bookId);
    public Res<IEnumerable<Author>> FindBookAuthors(Guid bookId)=> FindBookWithAuthors(bookId).Map(b => b.Authors.ToRes());

    public Res<Book> GetBook(Guid bookId) => Books.Untrack().FirstOrDefault(x => x.Id == bookId);
    public Res<Book> GetBookWithAuthors(Guid bookId) => Books.Untrack().WithAuthors().FirstOrDefault(x => x.Id == bookId);
    public Res<IEnumerable<Author>> GetBookAuthors(Guid bookId) => GetBookWithAuthors(bookId).Map(b => b.Authors.ToRes());
    #endregion Access

    public Res<Book> SaveNewBook(PostBook postBook)
    {
        var valRes = postBook.SimpleValidate();
        return valRes.IsValid
            ? SaveBook(postBook.ToBook())
            : valRes.Error;
    }

    public Res<Book> SaveBook(Guid bookId, PutBook putBook)
    {
        var bookRes = FindBook(bookId);
        var valRes = putBook.SimpleValidate();

        if (bookRes.IsFail && valRes.IsInvalid) return bookRes.Error.Concat(valRes.Error);
        else if (bookRes.IsFail) return bookRes.Error;
        else if (valRes.IsInvalid) return valRes.Error;
        else return bookRes.Match(
            book => ReplaceBookData(book, putBook.ToBook()),
            _ => SaveNewBookWithId(bookId, putBook));
    }

    public Res<Book> UpdateBook(Guid bookId, PatchBook patchBook)
    {
        var bookRes = FindBook(bookId);
        var valRes = patchBook.SimpleValidate();

        if (bookRes.IsFail && valRes.IsInvalid) return bookRes.Error.Concat(valRes.Error);
        else if (bookRes.IsFail) return bookRes.Error;
        else if (valRes.IsInvalid) return valRes.Error;
        else return bookRes.Match(
            book => UpdateBook(book, patchBook),
            err => err);
    }

    public Res DeleteBook(Guid bookId)
    {
        var bookRes = FindBook(bookId);
        return bookRes.Match(
            book =>
            {
                DbContext.Books.Remove(book);
                DbContext.SaveChanges();
                return Res.Ok();
            },
            err => err);
    }

    public Res AddAuthorToBook(Guid bookId, Guid authorId)
    {
        var bookRes = FindBookWithAuthors(bookId);
        var authorRes = DbContext.Authors
            .Include(a => a.Books)
            .FirstOrDefault(a => a.Id == authorId)
            .ToRes(ifNull: () => new NotFoundError());

        if (bookRes.IsFail && authorRes.IsFail) return bookRes.Error.Concat(authorRes.Error);
        else if (bookRes.IsFail && authorRes.IsOk) return bookRes.Error;
        else if (bookRes.IsOk && authorRes.IsFail) return authorRes.Error;
        bookRes.Value.AddAuthor(authorRes.Value);
        DbContext.SaveChanges();
        return Res.Ok();
    }

    public Res RemoveAuthorFromBook(Guid bookId, Guid authorId)
    {
        var bookRes = FindBookWithAuthors(bookId);
        var authorRes = DbContext.Authors
            .Include(a => a.Books)
            .FirstOrDefault(a => a.Id == authorId)
            .ToRes(ifNull: () => new NotFoundError());

        if (bookRes.IsFail && authorRes.IsFail) return bookRes.Error.Concat(authorRes.Error);
        else if (bookRes.IsFail && authorRes.IsOk) return bookRes.Error;
        else if (bookRes.IsOk && authorRes.IsFail) return authorRes.Error;
        bookRes.Value.RemoveAuthor(authorRes.Value);
        DbContext.SaveChanges();
        return Res.Ok();
    }


    Res<Book> SaveNewBookWithId(Guid bookId, PutBook putBook)
    {
        var book = new Book(bookId, putBook.Title, putBook.Edition, putBook.Price);
        DbContext.Books.Add(book);
        DbContext.SaveChanges();
        return book;
    }

    Res<Book> SaveBook(Book book)
    {
        var entry = DbContext.Books.Add(book);
        DbContext.SaveChanges();
        return entry.Entity;
    }

    Res<Book> UpdateBook(Book book, PatchBook patchBook)
    {
        book.Update(patchBook);
        DbContext.SaveChanges();
        return book;
    }

    Res<Book> ReplaceBookData(Book book, Book newBook)
    {
        book.Title = newBook.Title;
        book.Edition = newBook.Edition;
        book.Price = newBook.Price;

        book.RemoveAuthors(book.Authors);
        book.AddAuthors(newBook.Authors);

        DbContext.SaveChanges();

        return book;
    }
}
