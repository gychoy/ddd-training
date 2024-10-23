using eShop.Catalog.Application.Products.Models;
using eShop.Catalog.Application.Products.Queries;
using HotChocolate.Pagination;

namespace eShop.Catalog.Application.Products.Contracts;

public interface IProductsDataLoader
{
    Task<Page<ProductDto>> LoadAsync(
        PagingArguments pagingArgs,
        ProductFilter? filter = null,
        CancellationToken cancellationToken = default);
}