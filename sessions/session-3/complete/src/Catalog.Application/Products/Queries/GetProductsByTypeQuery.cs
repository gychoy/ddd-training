using eShop.Catalog.Application.Common.Contracts;
using eShop.Catalog.Application.Products.Contracts;
using eShop.Catalog.Application.Products.Models;
using GreenDonut.Selectors;
using HotChocolate.Pagination;
using MediatR;

namespace eShop.Catalog.Application.Products.Queries;

public sealed record GetProductsByTypeQuery(
    int TypeId,
    PagingArguments PagingArguments)
    : IQuery<Page<ProductDto>?>;

public sealed class GetProductsByTypeQueryHandler(
    IProductBatchingContext batching)
    : IRequestHandler<GetProductsByTypeQuery, Page<ProductDto>?>
{
    public Task<Page<ProductDto>?> Handle(
        GetProductsByTypeQuery request,
        CancellationToken cancellationToken)
    {
        return batching.ProductsByType
            .WithPagingArguments(request.PagingArguments)
            .LoadAsync(request.TypeId, cancellationToken);
    }
}