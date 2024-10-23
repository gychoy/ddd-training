using eShop.Catalog.Application.Brands.Services;
using eShop.Catalog.Application.Products.Services;
using eShop.Catalog.Application.ProductTypes.Services;
using Microsoft.Extensions.Hosting;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class OrderingApplicationServiceCollectionExtensions
{
    public static IHostApplicationBuilder AddApplicationLayer(
        this IHostApplicationBuilder builder)
    {
        builder.Services.AddApplicationLayer();
        return builder;
    }

    internal static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddScoped<BrandService>();
        services.AddScoped<ProductService>();
        services.AddScoped<ProductTypeService>();
        return services;
    }
    
}
