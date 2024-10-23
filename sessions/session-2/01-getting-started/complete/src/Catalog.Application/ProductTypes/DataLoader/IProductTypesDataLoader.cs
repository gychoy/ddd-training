using eShop.Catalog.Entities.ProductTypes;
using HotChocolate.Pagination;

namespace eShop.Catalog.Application.ProductTypes.DataLoader;

public interface IProductTypesDataLoader
{
    Task<Page<ProductType>> LoadAsync(
        PagingArguments pagingArgs,
        CancellationToken cancellationToken = default);
}