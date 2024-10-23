using eShop.Catalog.Application.Products.Contracts;
using eShop.Catalog.Application.Products.Models;
using eShop.Catalog.Application.Products.Queries;
using GreenDonut;
using HotChocolate.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace eShop.Catalog.Infrastructure.DataLoader;

internal sealed class ProductBatchingContext(
    IServiceProvider services)
    : IProductBatchingContext
{
    public IProductsDataLoader Products => services.GetRequiredService<IProductsDataLoader>();

    public IProductByIdDataLoader ProductById =>
        services.GetRequiredService<IProductByIdDataLoader>();

    public IProductsByBrandDataLoader ProductsByBrand =>
        services.GetRequiredService<IProductsByBrandDataLoader>();

    public IProductsByTypeDataLoader ProductsByType =>
        services.GetRequiredService<IProductsByTypeDataLoader>();
}

internal static class ProductDataLoader
{
    [DataLoader]
    public static Task<Dictionary<int, ProductDto>> GetProductByIdAsync(
        IReadOnlyList<int> ids,
        CatalogContext context,
        CancellationToken cancellationToken)
        => context.Products
            .AsNoTracking()
            .Where(x => ids.Contains(x.Id))
            .Select(x => new ProductDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Price = x.Price,
                ImageFileName = x.ImageFileName,
                BrandId = x.BrandId,
                TypeId = x.TypeId,
                AvailableStock = x.AvailableStock,
                OnReorder = x.OnReorder,
                RestockThreshold = x.RestockThreshold,
                MaxStockThreshold = x.MaxStockThreshold,
            })
            .ToDictionaryAsync(x => x.Id, cancellationToken);

    [DataLoader]
    public static async Task<Dictionary<int, Page<ProductDto>>> GetProductsByBrandAsync(
        IReadOnlyList<int> brandIds,
        PagingArguments pagingArgs,
        CatalogContext context,
        CancellationToken cancellationToken)
        => await context.Products
            .AsNoTracking()
            .Where(x => brandIds.Contains(x.BrandId))
            .Select(x => new ProductDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Price = x.Price,
                ImageFileName = x.ImageFileName,
                BrandId = x.BrandId,
                TypeId = x.TypeId,
                AvailableStock = x.AvailableStock,
                OnReorder = x.OnReorder,
                RestockThreshold = x.RestockThreshold,
                MaxStockThreshold = x.MaxStockThreshold,
            })
            .OrderBy(t => t.Name)
            .ThenBy(t => t.Id)
            .ToBatchPageAsync(x => x.BrandId, pagingArgs, cancellationToken);

    [DataLoader]
    public static async Task<Dictionary<int, Page<ProductDto>>> GetProductsByTypeAsync(
        IReadOnlyList<int> typeIds,
        PagingArguments pagingArgs,
        CatalogContext context,
        CancellationToken cancellationToken)
        => await context.Products
            .AsNoTracking()
            .Where(x => typeIds.Contains(x.TypeId))
            .Select(x => new ProductDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Price = x.Price,
                ImageFileName = x.ImageFileName,
                BrandId = x.BrandId,
                TypeId = x.TypeId,
                AvailableStock = x.AvailableStock,
                OnReorder = x.OnReorder,
                RestockThreshold = x.RestockThreshold,
                MaxStockThreshold = x.MaxStockThreshold,
            })
            .OrderBy(t => t.Name)
            .ThenBy(t => t.Id)
            .ToBatchPageAsync(x => x.TypeId, pagingArgs, cancellationToken);
}

internal sealed class ProductsDataLoader(CatalogContext context)
    : IProductsDataLoader
{
    public async Task<Page<ProductDto>> LoadAsync(
        PagingArguments pagingArgs,
        ProductFilter? filter = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.Products.AsNoTracking();

        if (filter?.BrandIds is { Count: > 0 } brandIds)
        {
            query = query.Where(p => brandIds.Contains(p.BrandId));
        }

        if (filter?.TypeIds is { Count: > 0 } typeIds)
        {
            query = query.Where(p => typeIds.Contains(p.TypeId));
        }

        var mapped = query.Select(x => new ProductDto
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            Price = x.Price,
            ImageFileName = x.ImageFileName,
            BrandId = x.BrandId,
            TypeId = x.TypeId,
            AvailableStock = x.AvailableStock,
            OnReorder = x.OnReorder,
            RestockThreshold = x.RestockThreshold,
            MaxStockThreshold = x.MaxStockThreshold,
        });
        
        mapped = mapped.OrderBy(t => t.Name).ThenBy(t => t.Id);

        return await mapped.ToPageAsync(pagingArgs, cancellationToken);
    }
}

internal sealed partial class ProductByIdDataLoader
    : IProductByIdDataLoader;

internal sealed partial class ProductsByBrandDataLoader
    : IProductsByBrandDataLoader;

internal sealed partial class ProductsByTypeDataLoader
    : IProductsByTypeDataLoader;