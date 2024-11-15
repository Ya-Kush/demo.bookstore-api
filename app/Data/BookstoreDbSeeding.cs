using App.DomainModels;

namespace App.Data;

public static class BookstoreSeedingExtensions
{
    #region SeedData
    readonly static Book[] _bookSeed = [
        Book.NewWithIdWithoutAuthors(id: new Guid(0x_00000000, 0000,0000,0000, 0,0,0,0,0,0,0),
            title: "Some Cool Book", edition: "3", price: 49.99),

        Book.NewWithIdWithoutAuthors(id: new Guid(0x_00000000, 0000,0000,0000, 0,0,0,0,0,0,1),
            title: "The Coolest Book", edition: "3", price: 99.99),
    ];
    readonly static Author[] _authorSeed = [
        Author.NewWithId(id: new Guid(0x_10000000, 0000,0000,0000, 0,0,0,0,0,0,0), "Some", "Cool", "Writer", []),
        Author.NewWithId(id: new Guid(0x_10000000, 0000,0000,0000, 0,0,0,0,0,0,1), "Some", "Cooler", "Writer", []),
        Author.NewWithId(id: new Guid(0x_10000000, 0000,0000,0000, 0,0,0,0,0,0,2), "The", "Coolest", "Writer", [])
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
            foreach (var a in _authorSeed)
                db.Authors.Add(a);
            db.SaveChanges();
        }
        return db;
    }

    static BookstoreDbContext SetRelationship(this BookstoreDbContext db)
    {
        var books = db.Books.AsEnumerable();

        foreach (var b in books.SkipLast(1)) b.AddAuthor(_authorSeed[0]);
        books.First().AddAuthor(_authorSeed[1]);
        books.Last().AddAuthor(_authorSeed[^1]);

        db.SaveChanges();
        return db;
    }
}
