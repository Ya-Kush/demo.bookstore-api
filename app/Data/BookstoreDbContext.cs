using App.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace App.Data;

public sealed class BookstoreDbContext(DbContextOptions<BookstoreDbContext> options) : DbContext(options)
{
    public DbSet<Author> Authors { get; private set; } = null!;
    public DbSet<Book> Books { get; private set; } = null!;
    public DbSet<Publisher> Publishers { get; private set; } = null!;

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
