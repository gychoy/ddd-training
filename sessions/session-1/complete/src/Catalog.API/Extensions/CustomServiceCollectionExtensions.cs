using eShop.Catalog.Application.Common.Contracts;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class CustomServiceCollectionExtensions
{
    public static IHostApplicationBuilder AddApiLayer(
        this IHostApplicationBuilder builder)
    {
        builder.Services.AddCors();
        return builder;
    }
    
    public static async Task SeedDataAsync(this WebApplication app) 
        => await app.Services.GetRequiredService<IDataSeeder>().SeedAsync();
}