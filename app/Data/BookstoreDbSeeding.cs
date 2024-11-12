using App.DomainModels;

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

    static bool _hasSeeded = false;

    public static WebApplication PopulateBookstore(this WebApplication bldr)
    {
        if (_hasSeeded) return bldr;

        using var scope = bldr.Services.CreateScope();
        using var bookstore = scope.ServiceProvider.GetRequiredService<BookstoreDbContext>();

        bookstore
            .PopulateWithBooks()
            .PopulateWithAuthor()
            .SetRelationship();

        _hasSeeded = true;
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

    static BookstoreDbContext PopulateWithAuthor(this BookstoreDbContext db)
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
}