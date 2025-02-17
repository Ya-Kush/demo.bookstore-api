using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Store.Data.Models.Configurators;

public sealed class AuthorEntityConfigurator : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder) => builder.Configure();
}

public static class AuthorEntityConfiguratorExtensions
{
    public static EntityTypeBuilder<Author> Configure(this EntityTypeBuilder<Author> authorBuilder)
    {
        authorBuilder.ToTable("Authors");

        authorBuilder.HasKey(x => x.Id).HasName("Id");
        authorBuilder.Property(x => x.Id).ValueGeneratedNever().IsRequired();

        authorBuilder.HasMany(x => x.Books).WithMany(x => x.Authors);

        return authorBuilder;
    }
}