using System.Linq.Expressions;
using eShop.Catalog.Entities.Brands;
using HotChocolate.Pagination;

namespace eShop.Catalog.Application.Brands.Contracts;

public interface IBrandsDataLoader
{
    Task<Page<Brand>> LoadAsync(
        PagingArguments pagingArgs,
        CancellationToken cancellationToken = default);
}