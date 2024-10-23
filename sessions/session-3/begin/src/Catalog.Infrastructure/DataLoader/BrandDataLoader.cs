using eShop.Catalog.Application.Brands;
using eShop.Catalog.Application.Brands.Contracts;
using GreenDonut;
using HotChocolate.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace eShop.Catalog.Infrastructure.DataLoader;

internal sealed class BrandBatchingContext(
    IServiceProvider services)
    : IBrandBatchingContext
{
    public IBrandsDataLoader Brands => services.GetRequiredService<IBrandsDataLoader>();
    public IBrandByIdDataLoader BrandById => services.GetRequiredService<IBrandByIdDataLoader>();

    public IBrandByNameDataLoader BrandByName =>
        services.GetRequiredService<IBrandByNameDataLoader>();
}

internal static class BrandDataLoader
{
    [DataLoader]
    public static Task<Dictionary<int, BrandDto>> GetBrandByIdAsync(
        IReadOnlyList<int> ids,
        CatalogContext context,
        CancellationToken cancellationToken)
        => context.Brands
            .AsNoTracking()
            .Where(x => ids.Contains(x.Id))
            .Select(x => new BrandDto { Id = x.Id, Name = x.Name })
            .ToDictionaryAsync(x => x.Id, cancellationToken);

    [DataLoader]
    public static Task<Dictionary<string, BrandDto>> GetBrandByNameAsync(
        IReadOnlyList<string> names,
        CatalogContext context,
        CancellationToken cancellationToken)
        => context.Brands
            .AsNoTracking()
            .Where(x => names.Contains(x.Name))
            .Select(x => new BrandDto { Id = x.Id, Name = x.Name })
            .ToDictionaryAsync(x => x.Name, cancellationToken);
}

internal sealed class BrandsDataLoader(CatalogContext context)
    : IBrandsDataLoader
{
    public async Task<Page<BrandDto>> LoadAsync(
        PagingArguments pagingArgs,
        CancellationToken cancellationToken = default)
        => await context.Brands
            .AsNoTracking()
            .Select(x => new BrandDto { Id = x.Id, Name = x.Name })
            .OrderBy(t => t.Name)
            .ThenBy(t => t.Id)
            .ToPageAsync(pagingArgs, cancellationToken);
}

internal sealed partial class BrandByIdDataLoader
    : IBrandByIdDataLoader;

internal sealed partial class BrandByNameDataLoader
    : IBrandByNameDataLoader;