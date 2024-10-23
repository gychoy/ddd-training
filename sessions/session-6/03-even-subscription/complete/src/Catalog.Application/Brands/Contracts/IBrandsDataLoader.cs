using HotChocolate.Pagination;

namespace eShop.Catalog.Application.Brands.Contracts;

public interface IBrandsDataLoader
{
    Task<Page<BrandDto>> LoadAsync(
        PagingArguments pagingArgs,
        CancellationToken cancellationToken = default);
}