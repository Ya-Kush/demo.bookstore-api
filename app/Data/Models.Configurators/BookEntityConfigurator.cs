using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Data.Models.Configurators;

public sealed class BookEntityConfigurator : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder) => builder.Configure();
}

public static class BookEntityConfiguratorExtensions
{
    public static EntityTypeBuilder<Book> Configure(this EntityTypeBuilder<Book> bookBuilder)
    {
        bookBuilder.ToTable("Books");

        bookBuilder.HasKey(x => x.Id).HasName("Id");
        bookBuilder.Property(x => x.Id).ValueGeneratedNever().IsRequired();

        bookBuilder.HasMany(x => x.Authors).WithMany(x => x.Books);
        bookBuilder.Navigation(x => x.Publisher).AutoInclude();

        return bookBuilder;
    }
}
