using eShop.Catalog.Application.Common.Contracts;
using eShop.Catalog.Application.ProductTypes.DataLoader;
using eShop.Catalog.Application.ProductTypes.Models;
using MediatR;

namespace eShop.Catalog.Application.ProductTypes.Queries;

public sealed record GetProductTypeByIdQuery(int Id) 
    : IQuery<ProductTypeDto?>;

public sealed class GetProductTypeByIdQueryHandler(
    IProductTypeBatchingContext batching)
    : IRequestHandler<GetProductTypeByIdQuery, ProductTypeDto?>
{
    public async Task<ProductTypeDto?> Handle(
        GetProductTypeByIdQuery request,
        CancellationToken cancellationToken)
        => await batching.ProductTypeById.LoadAsync(request.Id, cancellationToken);
}