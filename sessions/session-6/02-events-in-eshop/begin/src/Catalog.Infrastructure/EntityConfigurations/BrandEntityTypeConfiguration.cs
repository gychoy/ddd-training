using eShop.Catalog.Entities.Brands;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eShop.Catalog.Infrastructure.EntityConfigurations;

internal sealed class BrandEntityTypeConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder
            .ToTable("Brands")
            .HasKey(t => t.Id);

        builder
            .Property(t => t.Id)
            .ValueGeneratedOnAdd();

        builder
            .Property(t => t.Name)
            .HasMaxLength(100);

        builder
            .HasIndex(t => t.Name, "IX_Brands_Name")
            .IsUnique();

        builder.Ignore(t => t.Events);
    }
}