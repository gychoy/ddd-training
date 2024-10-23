using eShop.Catalog.Application.Products.Contracts;
using eShop.Catalog.Application.Products.Services;
using eShop.Catalog.Entities.Products;
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
    public IProductByIdDataLoader ProductById => services.GetRequiredService<IProductByIdDataLoader>();
    public IProductsByBrandDataLoader ProductsByBrand => services.GetRequiredService<IProductsByBrandDataLoader>();
    public IProductsByTypeDataLoader ProductsByType => services.GetRequiredService<IProductsByTypeDataLoader>();
}

internal static class ProductDataLoader
{
    [DataLoader]
    public static Task<Dictionary<int, Product>> GetProductByIdAsync(
        IReadOnlyList<int> ids,
        CatalogContext context,
        CancellationToken cancellationToken)
        => context.Products
            .AsNoTracking()
            .Where(x => ids.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, cancellationToken);

    [DataLoader]
    public static async Task<Dictionary<int, Page<Product>>> GetProductsByBrandAsync(
        IReadOnlyList<int> brandIds,
        PagingArguments pagingArgs,
        CatalogContext context,
        CancellationToken cancellationToken)
        => await context.Products
            .AsNoTracking()
            .Where(x => brandIds.Contains(x.BrandId))
            .OrderBy(t => t.Name)
            .ThenBy(t => t.Id)
            .ToBatchPageAsync(x => x.BrandId, pagingArgs, cancellationToken);
    
    [DataLoader]
    public static async Task<Dictionary<int, Page<Product>>> GetProductsByTypeAsync(
        IReadOnlyList<int> typeIds,
        PagingArguments pagingArgs,
        CatalogContext context,
        CancellationToken cancellationToken)
        => await context.Products
            .AsNoTracking()
            .Where(x => typeIds.Contains(x.TypeId))
            .OrderBy(t => t.Name)
            .ThenBy(t => t.Id)
            .ToBatchPageAsync(x => x.TypeId, pagingArgs, cancellationToken);
}

internal sealed class ProductsDataLoader(CatalogContext context)
    : IProductsDataLoader
{
    public async Task<Page<Product>> LoadAsync(
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
        
        query = query.OrderBy(t => t.Name).ThenBy(t => t.Id);

        return await query.ToPageAsync(pagingArgs, cancellationToken);
    }
}

internal sealed partial class ProductByIdDataLoader
    : IProductByIdDataLoader;

internal sealed partial class ProductsByBrandDataLoader
    : IProductsByBrandDataLoader;

internal sealed partial class ProductsByTypeDataLoader
    : IProductsByTypeDataLoader;

