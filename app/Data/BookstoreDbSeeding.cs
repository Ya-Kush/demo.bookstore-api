using App.DomainModels;
using Microsoft.EntityFrameworkCore;

namespace App.Data;

public static class BookstoreSeedingExtensions
{
    #region SeedData
    readonly static Book[] _bookSeed = [
        Book.New(title: "Some Cool Book", edition: "3", price: 49.99,
            authors: [],
            publisher: "Some Cool Publisher", released: "2020/06/09"),

        Book.New(title: "The Coolest Book", edition: "3", price: 99.99,
            authors: [],
            publisher: "The Coolest Publisher", released: DateTime.UtcNow.ToString("yyyy:/MM:/dd")),
    ];
    readonly static Author[] _authorSeed = [
        Author.New("Some", "Cool", "Writer", []),
        Author.New("Some", "Cooler", "Writer", []),
        Author.New("The", "Coolest", "Writer", [])
    ];
    #endregion

    #region Simple Populating
    public static WebApplication PopulateBookstore(this WebApplication bldr)
    {
        using var db = bldr.Services.CreateScope().ServiceProvider.GetRequiredService<BookstoreDbContext>();
        db.PopulateWithBooks()
            .PopulateDbWithAuthor()
            .SetRelationship();
        return bldr;
    }

    static BookstoreDbContext PopulateWithBooks(this BookstoreDbContext db)
    {
        if (db.Books.Any() is false)
        {
            db.Books.AddRange(_bookSeed);
            db.SaveChanges();
        }
        return db;
    }

    static BookstoreDbContext PopulateDbWithAuthor(this BookstoreDbContext db)
    {
        if (db.Authors.Any() is false)
        {
            db.Authors.AddRange(_authorSeed);
            db.SaveChanges();
        }
        return db;
    }

    static BookstoreDbContext SetRelationship(this BookstoreDbContext db)
    {
        var books = db.Books.AsEnumerable();

        foreach (var b in books.SkipLast(1)) b.AddAuthors(_authorSeed[0]);
        books.First().AddAuthors(_authorSeed[1]);
        books.Last().AddAuthors(_authorSeed[^1]);

        db.SaveChanges();
        return db;
    }

    static DbSet<Book> WithAuthors(this DbSet<Book> bookSet)
    {
        var books = bookSet.AsEnumerable();

        foreach (var b in books.SkipLast(1)) b.AddAuthors(_authorSeed[0]);
        books.First().AddAuthors(_authorSeed[1]);
        books.Last().AddAuthors(_authorSeed[^1]);

        return bookSet;
    }

    static DbSet<Author> WithBooks(this DbSet<Author> authorSet)
    {
        var authors = authorSet.AsEnumerable();

        authors.First().AddBooks([.._bookSeed.SkipLast(1)]);
        authors.ElementAt(1).AddBooks(_bookSeed[0]);
        authors.Last().AddBooks(_bookSeed[^1]);

        return authorSet;
    }

    #endregion

    // Doesnt work. Maybe InMemoryDb doesnt support that
    #region Populating By EF Seeding Methods
    public static DbContextOptionsBuilder PopulateWithEverything(this DbContextOptionsBuilder bldr) => bldr
        .PopulateWithBook();

    static DbContextOptionsBuilder PopulateWithBook(this DbContextOptionsBuilder bldr)
    {
        bldr.UseSeeding((db, _) =>
            {
                if (db.Set<Book>().Any()) return;
                db.Set<Book>().AddRange(_bookSeed);
                db.SaveChanges();
            });
        bldr.UseAsyncSeeding(async (db, _, cancel) =>
            {
                if (await db.Set<Book>().AnyAsync(cancel)) return;
                await db.Set<Book>().AddRangeAsync(_bookSeed, cancel);
                await db.SaveChangesAsync(cancel);
            });
        return bldr;
    }
    #endregion
}