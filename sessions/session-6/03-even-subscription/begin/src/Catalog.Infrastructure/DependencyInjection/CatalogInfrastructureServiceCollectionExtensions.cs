using eShop.Catalog.Application.Brands.Contracts;
using eShop.Catalog.Application.Common.Contracts;
using eShop.Catalog.Application.Products.Contracts;
using eShop.Catalog.Application.ProductTypes.DataLoader;
using eShop.Catalog.Common;
using eShop.Catalog.Entities.Brands;
using eShop.Catalog.Entities.Products;
using eShop.Catalog.Entities.ProductTypes;
using eShop.Catalog.Infrastructure;
using eShop.Catalog.Infrastructure.DataLoader;
using eShop.Catalog.Infrastructure.Migrations;
using eShop.Catalog.Infrastructure.Repositories;
using eShop.EventBus;
using Microsoft.Extensions.Hosting;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class CatalogInfrastructureServiceCollectionExtensions
{
    public static IHostApplicationBuilder AddInfrastructureLayer(
        this IHostApplicationBuilder builder,
        string connectionName = "CatalogDB")
    {
        builder.AddNpgsqlDbContext<CatalogContext>(connectionName);
        builder.AddNpgsqlDataSource(connectionName);
        builder.Services.AddInfrastructureLayer();
        builder.AddRabbitMQEventBus();
        return builder;
    }

    private static void AddInfrastructureLayer(this IServiceCollection services)
    {
        services.AddDataLoader();

        services.AddScoped<IProductByIdDataLoader>(s => s.GetRequiredService<ProductByIdDataLoader>());
        services.AddScoped<IProductsByBrandDataLoader>(s => s.GetRequiredService<ProductsByBrandDataLoader>());
        services.AddScoped<IProductsByTypeDataLoader>(s => s.GetRequiredService<ProductsByTypeDataLoader>());
        services.AddScoped<IProductsDataLoader, ProductsDataLoader>();
        services.AddScoped<IProductBatchingContext, ProductBatchingContext>();

        services.AddScoped<IBrandByIdDataLoader>(s => s.GetRequiredService<BrandByIdDataLoader>());
        services.AddScoped<IBrandByNameDataLoader>(s => s.GetRequiredService<BrandByNameDataLoader>());
        services.AddScoped<IBrandsDataLoader, BrandsDataLoader>();
        services.AddScoped<IBrandBatchingContext, BrandBatchingContext>();
        
        services.AddScoped<IProductTypeByIdDataLoader>(s => s.GetRequiredService<ProductTypeByIdDataLoader>());
        services.AddScoped<IProductTypeByNameDataLoader>(s => s.GetRequiredService<ProductTypeByNameDataLoader>());
        services.AddScoped<IProductTypesDataLoader, ProductTypesDataLoader>();
        services.AddScoped<IProductTypeBatchingContext, ProductTypeBatchingContext>();
        
        services.AddScoped<IProductTypeRepository, ProductTypeRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IBrandRepository, BrandRepository>();
        
        services.AddSingleton<IDataSeeder, CatalogContextSeed>();

        services.AddScoped<CatalogDataContext>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<CatalogDataContext>());
        services.AddScoped<ICatalogDataContext>(sp => sp.GetRequiredService<CatalogDataContext>());

        services.AddSingleton(TimeProvider.System);
        
        services.AddPostgresIntegrationEvents<CatalogContext>();
    }
}