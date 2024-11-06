using App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Data;

public sealed class BookstoreDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Book> Books { get; private set; }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ConfigureBook();
    }
}

public static class EntityConfiguratorsExtentions
{
    public static EntityTypeBuilder<Book> ConfigureBook(this ModelBuilder modelBuilder)
    {
        var bookBuilder = modelBuilder.Entity<Book>();

        bookBuilder.ToTable("Books");

        bookBuilder.Property(x => x.Id)
            .HasConversion(to => to.Value, from => new(from));

        return bookBuilder;
    }
}

public static class BookstoreSeedingExtentions
{
    readonly static Book[] _bookSeed = [
        Book.New(title: "Some Cool Book", edition: "3", price: 99.99,
            authors: ["Some Cool Writer", "Some Cooler Writer"],
            publisher: "Some Cool Publisher", released: "2020/06/09"),

        Book.New(title: "The Coolest Book", edition: "3", price: 99.99,
            authors: ["The Coolest Writer"],
            publisher: "The Coolest Publisher", released: DateTime.UtcNow.ToString("yyyy:/MM:/dd")),
    ];

    public static DbContextOptionsBuilder PopulateWithEverything(this DbContextOptionsBuilder bldr)
    {
        return bldr.PopulateWithBook();
    }

    public static DbContextOptionsBuilder PopulateWithBook(this DbContextOptionsBuilder bldr)
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
}