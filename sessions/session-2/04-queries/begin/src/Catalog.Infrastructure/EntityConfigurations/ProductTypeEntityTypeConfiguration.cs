using eShop.Catalog.Entities.ProductTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eShop.Catalog.Infrastructure.EntityConfigurations;

internal sealed class ProductTypeEntityTypeConfiguration : IEntityTypeConfiguration<ProductType>
{
    public void Configure(EntityTypeBuilder<ProductType> builder)
    {
        builder
            .ToTable("ProductTypes")
            .HasKey(t => t.Id);

        builder
            .Property(t => t.Id)
            .ValueGeneratedOnAdd();

        builder
            .Property(cb => cb.Name)
            .HasMaxLength(100);

        builder
            .HasIndex(t => t.Name, "IX_ProductTypes_Name")
            .IsUnique();
    }
}
