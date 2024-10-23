using eShop.Catalog.Application.Products.Services;
using eShop.Catalog.Entities.Products;
using HotChocolate.Pagination;

namespace eShop.Catalog.Application.Products.Contracts;

public interface IProductsDataLoader
{
    Task<Page<Product>> LoadAsync(
        PagingArguments pagingArgs,
        ProductFilter? filter = null,
        CancellationToken cancellationToken = default);
}
