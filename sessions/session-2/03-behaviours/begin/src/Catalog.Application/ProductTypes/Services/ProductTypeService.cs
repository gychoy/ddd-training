using eShop.Catalog.Application.ProductTypes.DataLoader;
using eShop.Catalog.Entities.ProductTypes;
using HotChocolate.Pagination;

namespace eShop.Catalog.Application.ProductTypes.Services;

public sealed class ProductTypeService(IProductTypeBatchingContext batching)
{
    public async Task<ProductType?> GetProductTypeByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
        => await batching.ProductTypeById.LoadAsync(id, cancellationToken);

    public async Task<ProductType?> GetProductTypeByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
        => await batching.ProductTypeByName.LoadAsync(name, cancellationToken);

    public async Task<Page<ProductType>> GetProductTypesAsync(
        PagingArguments pagingArguments,
        CancellationToken cancellationToken = default)
        => await batching.ProductTypes.LoadAsync(pagingArguments, cancellationToken);
}