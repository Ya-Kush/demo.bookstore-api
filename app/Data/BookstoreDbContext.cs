using App.Data.Models;
using Microsoft.EntityFrameworkCore;
#nullable disable

namespace App.Data;

public sealed class BookstoreDbContext(DbContextOptions<BookstoreDbContext> options) : DbContext(options)
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
