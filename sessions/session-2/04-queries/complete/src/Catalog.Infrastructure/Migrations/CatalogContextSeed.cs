using System.Text.Json;
using eShop.Catalog.Application.Common.Contracts;
using eShop.Catalog.Entities.Brands;
using eShop.Catalog.Entities.Products;
using eShop.Catalog.Entities.ProductTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace eShop.Catalog.Infrastructure.Migrations;

public sealed class CatalogContextSeed(
    IServiceProvider services)
    : IDataSeeder
{
    public async Task SeedAsync()
    {
        const string fileName = "eShop.Catalog.Infrastructure.Migrations.catalog.json";
        
        string sourceJson;
        var assembly = typeof(CatalogContextSeed).Assembly;
        await using (var stream = assembly.GetManifestResourceStream(fileName)!)
        using (var reader = new StreamReader(stream))
        {
            sourceJson = await reader.ReadToEndAsync();
        }
        
        await using var scope = services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<CatalogContext>();

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        if (!context.Products.Any())
        {
            var sourceItems = JsonSerializer.Deserialize<ProductEntry[]>(sourceJson)!;

            var names = new HashSet<string>();

            foreach (var source in sourceItems)
            {
                if (names.Add(source.Brand))
                {
                    var brand = new Brand { Name = source.Brand };
                    context.Brands.Add(brand);
                }

                if (names.Add(source.Type))
                {
                    var type = new ProductType { Name = source.Type };
                    context.ProductTypes.Add(type);
                }
            }

            await context.SaveChangesAsync();

            var brandIdsByName = await context.Brands.ToDictionaryAsync(x => x.Name, x => x.Id);
            var typeIdsByName = await context.ProductTypes.ToDictionaryAsync(x => x.Name, x => x.Id);

            foreach (var source in sourceItems)
            {
                if (names.Add(source.Name + "_" + source.Brand))
                {
                    var product = new Product 
                    {
                        Name = source.Name,
                        Description = source.Description,
                        Price = source.Price,
                        BrandId = brandIdsByName[source.Brand],
                        TypeId = typeIdsByName[source.Type],
                        AvailableStock = 0,
                        RestockThreshold = int.MaxValue
                    };
                    context.Products.Add(product);
                }
            }

            await context.SaveChangesAsync();
        }
    }

    private sealed class ProductEntry
    {
        public required int Id { get; set; }
        public required string Type { get; set; }
        public required string Brand { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required decimal Price { get; set; }
    }
}
