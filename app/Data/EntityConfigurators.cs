using App.DomainModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Data;

public static class EntityConfiguratorsExtensions
{
    public static EntityTypeBuilder<Book> Configure(this EntityTypeBuilder<Book> bookBuilder)
    {
        bookBuilder.ToTable("Books");

        bookBuilder.HasKey(x => x.Id).HasName("Id");

        bookBuilder.HasMany(x => x.Authors).WithMany(x => x.Books);

        return bookBuilder;
    }

    public static EntityTypeBuilder<Author> Configure(this EntityTypeBuilder<Author> authorBuilder)
    {
        authorBuilder.ToTable("Authors");

        authorBuilder.HasKey(x => x.Id).HasName("Id");

        authorBuilder.HasMany(x => x.Books).WithMany(x => x.Authors);

        return authorBuilder;
    }
}
