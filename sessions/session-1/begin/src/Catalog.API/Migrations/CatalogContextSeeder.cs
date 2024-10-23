using System.Text.Json;
using eShop.Catalog.Data;
using eShop.Catalog.Models;
using Microsoft.EntityFrameworkCore;
using Path = System.IO.Path;

namespace eShop.Catalog.Migrations;

public sealed class CatalogContextSeeder(
    IWebHostEnvironment env,
    IServiceProvider services)
    : IDataSeeder
{
    public async Task SeedAsync()
    {
        var contentRootPath = env.ContentRootPath;

        await using var scope = services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<CatalogContext>();
        
        await context.Database.EnsureCreatedAsync();

        if (!await context.Products.AnyAsync())
        {
            var sourcePath = Path.Combine(contentRootPath, "Migrations", "catalog.json");
            var sourceJson = await File.ReadAllTextAsync(sourcePath);
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