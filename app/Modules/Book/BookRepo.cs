using App.Common;
using App.Data;
using App.Data.Models;
using App.Endpoints.Models;
using App.Endpoints.Models.Validators;
using Microsoft.EntityFrameworkCore;
using static App.Common.Res;

namespace App.Endpoints.Services;

public sealed class BookRepo(BookstoreDbContext bookstoreDbContext)
{
    BookstoreDbContext DbContext { get; } = bookstoreDbContext;

    public IQueryable<Book> Books => DbContext.Books;
    public IQueryable<Book> UntrackedBooks => Books.AsNoTracking();

    public IRes<Book> FindUntracked(Guid id) => GetUntrackedBook(id).IfNull(NotFound, Ok);
    public IRes<Book> Find(Guid id) => GetBook(id).IfNull(NotFound, Ok);
    public IRes<IEnumerable<Author>> FindBookAuthors(Guid id)
        => UntrackedBooks
            .Include(x => x.Authors)
            .FirstOrDefault(x => x.Id == id)?.Authors
                .IfNull(NotFound, Ok)!;

    Book? GetUntrackedBook(Guid id) => UntrackedBooks.FirstOrDefault(x => x.Id == id);
    Book? GetBook(Guid id) => DbContext.Books.FirstOrDefault(x => x.Id == id);

    public IRes<Book> SaveNewBook(PostBook postBook)
        {
            var val = postBook.SimpleValidate();
            return val.IsValid
                ? SaveBook(postBook.ToBook())
                : Error<Book>(val.Errors);
        }

    public IRes<Book> SaveBook(Guid id, PutBook putBook)
    {
        var book = GetBook(id);
        var val = putBook.SimpleValidate();
        if (val.IsInvalid) return Error<Book>(val.Errors);

        return book.IfNull(
            a => SaveNewBookWithId(id, putBook),
            a => ReplaceBook(a, putBook.ToBook()));
    }

    public IRes<Book> UpdateBook(Guid id, PatchBook patchBook)
    {
        var book = GetBook(id);
        var val = patchBook.SimpleValidate();
        var err = book is null ? ["Book Not Found", ..val.Errors] : val.Errors;

        if (err.Any()) return Error(book, err);

        return UpdateBook(book!, patchBook);
    }

    public IRes<Book> DeleteBook(Guid id)
    {
        var book = GetBook(id);
        if (book is null) return Fail(book);

        DbContext.Books.Remove(book);
        DbContext.SaveChanges();

        return Ok(book);
    }


    IRes<Book> SaveNewBookWithId(Guid id, PutBook putBook)
    {
        var book = new Book(id, putBook.Title, putBook.Edition, putBook.Price);
        DbContext.Books.Add(book);
        DbContext.SaveChanges();
        return Ok(book);
    }

    IRes<Book> SaveBook(Book book)
    {
        var entry = DbContext.Books.Add(book);
        DbContext.SaveChanges();
        return Ok(entry.Entity);
    }

    IRes<Book> UpdateBook(Book book, PatchBook patchBook)
    {
        book.Update(patchBook);
        DbContext.SaveChanges();
        return Ok(book);
    }

    IRes<Book> ReplaceBook(Book book, Book newBook)
    {
        book.Title = newBook.Title;
        book.Edition = newBook.Edition;
        book.Price = newBook.Price;

        book.RemoveAuthors(book.Authors);
        book.AddAuthors(newBook.Authors);

        DbContext.SaveChanges();

        return Updated(book);
    }
}
