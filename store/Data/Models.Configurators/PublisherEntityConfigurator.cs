using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Store.Data.Models.Configurators;

public sealed class PublisherEntityConfigurator : IEntityTypeConfiguration<Publisher>
{
    public void Configure(EntityTypeBuilder<Publisher> builder) => builder.Configure();
}

public static class PublisherEntityConfiguratorExtensions
{
    public static EntityTypeBuilder<Publisher> Configure(this EntityTypeBuilder<Publisher> pubBuilder)
    {
        pubBuilder.ToTable("Publishers");

        pubBuilder.HasKey(p => p.Id).HasName("Id");
        pubBuilder.Property(p => p.Id).ValueGeneratedNever().IsRequired();

        pubBuilder.HasIndex(p => p.Name, "Index_Name").IsUnique();

        pubBuilder.HasMany(p => p.Books).WithOne(b => b.Publisher).OnDelete(DeleteBehavior.SetNull);

        return pubBuilder;
    }
}