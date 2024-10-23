using eShop.Catalog.Migrations;
using eShop.Catalog.Services;
using Microsoft.EntityFrameworkCore;
using eShop.Catalog.Data;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class CustomServiceCollectionExtensions
{
    public static IHostApplicationBuilder AddApplicationServices(
        this IHostApplicationBuilder builder)
    {
        builder.Services.AddApplicationServices(
            builder.Configuration.GetConnectionString("CatalogDB"));
        return builder;
    }
    
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        string? connectionString = null)
    {
        services
            .AddDbContextPool<CatalogContext>(
                o => o.UseNpgsql(connectionString));
        services
            .AddSingleton<IDataSeeder, CatalogContextSeeder>();

        services
            .AddScoped<BrandService>()
            .AddScoped<ProductService>()
            .AddScoped<ProductTypeService>();
        
        services.AddCors();
        
        return services;
    }

    public static async Task SeedDataAsync(this WebApplication app) 
        => await app.Services.GetRequiredService<IDataSeeder>().SeedAsync();
}