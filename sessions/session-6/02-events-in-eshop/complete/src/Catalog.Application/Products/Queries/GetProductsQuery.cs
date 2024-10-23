using eShop.Catalog.Application.Common.Contracts;
using eShop.Catalog.Application.Products.Contracts;
using eShop.Catalog.Application.Products.Models;
using eShop.Catalog.Entities.Products;
using HotChocolate.Pagination;
using MediatR;

namespace eShop.Catalog.Application.Products.Queries;

public sealed record GetProductsQuery(
    PagingArguments PagingArgs,
    ProductFilter? ProductFilter = null)
    : IQuery<Page<ProductDto>>;

public sealed class GetProductsQueryHandler(
    IProductBatchingContext batching)
    : IRequestHandler<GetProductsQuery, Page<ProductDto>>
{
    public async Task<Page<ProductDto>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
        => await batching.Products.LoadAsync(
            request.PagingArgs,
            request.ProductFilter,
            cancellationToken);
}