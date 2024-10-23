using eShop.Catalog.Application.Brands.Contracts;
using eShop.Catalog.Entities.Brands;
using HotChocolate.Pagination;

namespace eShop.Catalog.Application.Brands.Services;

public sealed class BrandService(
    IBrandBatchingContext batching)
{
    public async Task<Brand?> GetBrandByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
        => await batching.BrandById.LoadAsync(id, cancellationToken);

    public async Task<Brand?> GetBrandByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
        => await batching.BrandByName.LoadAsync(name, cancellationToken);

    public async Task<Page<Brand>> GetBrandsAsync(
        PagingArguments args,
        CancellationToken cancellationToken = default)
        => await batching.Brands.LoadAsync(args, cancellationToken);
}