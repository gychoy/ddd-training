using eShop.Catalog.Entities.Brands;
using eShop.Catalog.Entities.Products;
using eShop.Catalog.Entities.ProductTypes;
using eShop.Catalog.Infrastructure.EntityConfigurations;
using eShop.IntegrationEvents.EntityFramework.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace eShop.Catalog.Infrastructure;

/// <remarks>
/// Add migrations using the following command inside the 'Catalog.API' project directory:
///
/// dotnet ef migrations add --context CatalogContext [migration-name]
/// </remarks>
public class CatalogContext : DbContext
{
    public CatalogContext(
        DbContextOptions<CatalogContext> options,
        IConfiguration configuration)
        : base(options) { }

    public DbSet<Product> Products => Set<Product>();

    public DbSet<ProductType> ProductTypes => Set<ProductType>();

    public DbSet<Brand> Brands => Set<Brand>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new BrandEntityTypeConfiguration());
        builder.ApplyConfiguration(new ProductTypeEntityTypeConfiguration());
        builder.ApplyConfiguration(new ProductEntityTypeConfiguration());
        builder.UseIntegrationEvents();
    }
}
