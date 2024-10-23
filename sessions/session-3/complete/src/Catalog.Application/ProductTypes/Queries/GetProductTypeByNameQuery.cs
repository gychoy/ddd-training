using eShop.Catalog.Application.Common.Contracts;
using eShop.Catalog.Application.ProductTypes.DataLoader;
using eShop.Catalog.Application.ProductTypes.Models;
using MediatR;

namespace eShop.Catalog.Application.ProductTypes.Queries;

public sealed record GetProductTypeByNameQuery(string Name) 
    : IQuery<ProductTypeDto?>;

public sealed class GetProductTypeByNameQueryHandler(
    IProductTypeBatchingContext batching)
    : IRequestHandler<GetProductTypeByNameQuery, ProductTypeDto?>
{
    public async Task<ProductTypeDto?> Handle(
        GetProductTypeByNameQuery request,
        CancellationToken cancellationToken)
        => await batching.ProductTypeByName.LoadAsync(request.Name, cancellationToken);
}