using eShop.Catalog.Entities.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eShop.Catalog.Infrastructure.EntityConfigurations;

internal sealed class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder
            .ToTable("Products")
            .HasKey(t => t.Id);

        builder
            .Property(t => t.Id)
            .ValueGeneratedOnAdd();

        builder
            .Property(t => t.Name)
            .HasMaxLength(50);

        builder
            .Property(t => t.Description)
            .HasMaxLength(2048);

        builder
            .Property(t => t.ImageFileName)
            .HasMaxLength(256);

        builder
            .HasIndex(t => t.Name, "IX_Products_Name");
    }
}
