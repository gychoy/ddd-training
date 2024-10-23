using eShop.Catalog.Application.ProductTypes.DataLoader;
using eShop.Catalog.Entities.ProductTypes;
using GreenDonut;
using HotChocolate.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace eShop.Catalog.Infrastructure.DataLoader;

internal sealed class ProductTypeBatchingContext(
    IServiceProvider services)
    : IProductTypeBatchingContext
{
    public IProductTypesDataLoader ProductTypes => services.GetRequiredService<IProductTypesDataLoader>();
    public IProductTypeByIdDataLoader ProductTypeById => services.GetRequiredService<IProductTypeByIdDataLoader>();
    public IProductTypeByNameDataLoader ProductTypeByName => services.GetRequiredService<IProductTypeByNameDataLoader>();
}

internal static class ProductTypeDataLoader
{
    [DataLoader]
    public static Task<Dictionary<int, ProductType>> GetProductTypeByIdAsync(
        IReadOnlyList<int> ids,
        CatalogContext context,
        CancellationToken cancellationToken)
        => context.ProductTypes
            .AsNoTracking()
            .Where(x => ids.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, cancellationToken);
    
    [DataLoader]
    public static Task<Dictionary<string, ProductType>> GetProductTypeByNameAsync(
        IReadOnlyList<string> names,
        CatalogContext context,
        CancellationToken cancellationToken)
        => context.ProductTypes
            .AsNoTracking()
            .Where(x => names.Contains(x.Name))
            .ToDictionaryAsync(x => x.Name, cancellationToken);
}

internal sealed class ProductTypesDataLoader(CatalogContext context)
    : IProductTypesDataLoader
{
    public async Task<Page<ProductType>> LoadAsync(
        PagingArguments pagingArgs,
        CancellationToken cancellationToken = default)
        => await context.ProductTypes
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .ThenBy(t => t.Id)
            .ToPageAsync(pagingArgs, cancellationToken);
}

internal sealed partial class ProductTypeByIdDataLoader
    : IProductTypeByIdDataLoader;

internal sealed partial class ProductTypeByNameDataLoader
    : IProductTypeByNameDataLoader;
