using eShop.Catalog.Application.ProductTypes.Models;
using HotChocolate.Pagination;

namespace eShop.Catalog.Application.ProductTypes.DataLoader;

public interface IProductTypesDataLoader
{
    Task<Page<ProductTypeDto>> LoadAsync(
        PagingArguments pagingArgs,
        CancellationToken cancellationToken = default);
}