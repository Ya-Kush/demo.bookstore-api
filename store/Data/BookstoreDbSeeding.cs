using Store.Data.Models;

namespace Store.Data;

public static class BookstoreSeedingExtensions
{
    #region SeedData
    readonly static Author[] _authorSeed = [
        new(id: new Guid(0x_10000000, 0000,0000,0000, 0,0,0,0,0,0,0), "Some", "Cool", "Writer"),
        new(id: new Guid(0x_10000000, 0000,0000,0000, 0,0,0,0,0,0,1), "Some", "Cooler", "Writer"),
        new(id: new Guid(0x_10000000, 0000,0000,0000, 0,0,0,0,0,0,2), "The", "Coolest", "Writer"),
    ];
    readonly static Book[] _bookSeed = [
        new(id: new Guid(0x_00000000, 0000,0000,0000, 0,0,0,0,0,0,0),
            title: "Some Cool Book", edition: "3", price: 49.99),

        new(id: new Guid(0x_00000000, 0000,0000,0000, 0,0,0,0,0,0,1),
            title: "The Coolest Book", edition: "3", price: 99.99),
    ];
    readonly static Publisher[] _publishers = [
        new(id: new Guid(0x_00000000, 1000,0000,0000, 0,0,0,0,0,0,0), name: "Some Cool Publisher"),
    ];
    #endregion

    public static WebApplication PopulateBookstore(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        using var bookstore = scope.ServiceProvider.GetRequiredService<BookstoreDbContext>();

        bookstore
            .PopulateWithAuthor()
            .PopulateWithBooks()
            .PopelateWithPublishers()
            .SetRelationship()
            .SaveChanges();

        return app;
    }

    static BookstoreDbContext PopulateWithAuthor(this BookstoreDbContext db)
    {
        db.Authors.AddRange(_authorSeed);
        return db;
    }

    static BookstoreDbContext PopulateWithBooks(this BookstoreDbContext db)
    {
        db.Books.AddRange(_bookSeed);
        return db;
    }

    static BookstoreDbContext PopelateWithPublishers(this BookstoreDbContext db)
    {
        db.Publishers.AddRange(_publishers);
        return db;
    }

    static BookstoreDbContext SetRelationship(this BookstoreDbContext db)
    {
        var books = db.Books.Local;

        foreach (var b in books.SkipLast(1)) b.AddAuthor(_authorSeed[0]);
        books.First().AddAuthor(_authorSeed[1]);
        books.Last().AddAuthor(_authorSeed[^1]);

        // _publishers[0].AddBooks(books.Skip(1));
        foreach (var b in books.Skip(1))
            b.Publisher = _publishers[0];

        return db;
    }
}
