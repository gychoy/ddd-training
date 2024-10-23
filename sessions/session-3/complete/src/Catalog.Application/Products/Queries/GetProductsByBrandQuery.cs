using eShop.Catalog.Application.Common.Contracts;
using eShop.Catalog.Application.Products.Contracts;
using eShop.Catalog.Application.Products.Models;
using GreenDonut.Selectors;
using HotChocolate.Pagination;
using MediatR;

namespace eShop.Catalog.Application.Products.Queries;

public sealed record GetProductsByBrandQuery(
    int BrandId,
    PagingArguments PagingArguments)
    : IQuery<Page<ProductDto>?>;

public sealed class GetProductsByBrandQueryHandler(
    IProductBatchingContext batching)
    : IRequestHandler<GetProductsByBrandQuery, Page<ProductDto>?>
{
    public Task<Page<ProductDto>?> Handle(
        GetProductsByBrandQuery request,
        CancellationToken cancellationToken)
    {
        return batching.ProductsByBrand
            .WithPagingArguments(request.PagingArguments)
            .LoadAsync(request.BrandId, cancellationToken);
    }
}