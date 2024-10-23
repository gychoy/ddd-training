using eShop.Catalog.Application.Common.Contracts;
using eShop.Catalog.Application.ProductTypes.DataLoader;
using eShop.Catalog.Application.ProductTypes.Models;
using HotChocolate.Pagination;
using MediatR;

namespace eShop.Catalog.Application.ProductTypes.Queries;

public sealed record GetProductTypesQuery(PagingArguments PagingArgs)
    : IQuery<Page<ProductTypeDto>>;

public sealed class GetProductTypesQueryHandler(
    IProductTypeBatchingContext batching)
    : IRequestHandler<GetProductTypesQuery, Page<ProductTypeDto>>
{
    public async Task<Page<ProductTypeDto>> Handle(
        GetProductTypesQuery request,
        CancellationToken cancellationToken)
        => await batching.ProductTypes.LoadAsync(request.PagingArgs, cancellationToken);
}