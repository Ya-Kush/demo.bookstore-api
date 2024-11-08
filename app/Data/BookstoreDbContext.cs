using App.DomainModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Data;

public sealed class BookstoreDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Book> Books { get; private set; }
    public DbSet<Author> Authors { get; private set; }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Book>().Configure();
        modelBuilder.Entity<Author>().Configure();
    }
}

public static class EntityConfiguratorsExtensions
{
    public static EntityTypeBuilder<Book> Configure(this EntityTypeBuilder<Book> bookBuilder)
    {
        bookBuilder.ToTable("Books");

        bookBuilder.HasKey(x => x.Guid);

        bookBuilder.HasMany(x => x.Authors).WithMany(x => x.Books);

        return bookBuilder;
    }

    public static EntityTypeBuilder<Author> Configure(this EntityTypeBuilder<Author> authorBuilder)
    {
        authorBuilder.ToTable("Authors");

        authorBuilder.HasKey(x => x.Guid);

        authorBuilder.HasMany(x => x.Books).WithMany(x => x.Authors);

        return authorBuilder;
    }
}

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
    readonly static Author[] _authors = [
        Author.New("Some", "Cool", "Writer", []),
        Author.New("Some", "Cooler", "Writer", []),
        Author.New("The", "Coolest", "Writer", [])
    ];
    #endregion

    #region Simple Populating
    public static WebApplication PopulateBookstore(this WebApplication bldr)
    {
        using var scope = bldr.Services.CreateScope().ServiceProvider.GetRequiredService<BookstoreDbContext>()
            .PopulateWithBooks()
            .PopulateDbWithAuthor()
            ;
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
            db.Authors.AddRange(_authors);
            db.SaveChanges();
        }
        return db;
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