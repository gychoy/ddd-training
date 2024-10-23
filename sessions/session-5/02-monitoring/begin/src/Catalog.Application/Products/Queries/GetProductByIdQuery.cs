using eShop.Catalog.Application.Common.Contracts;
using eShop.Catalog.Application.Products.Contracts;
using eShop.Catalog.Application.Products.Models;
using MediatR;

namespace eShop.Catalog.Application.Products.Queries;

public sealed record GetProductByIdQuery(int Id) : IQuery<ProductDto?>;

public sealed class GetProductByIdQueryHandler(
    IProductBatchingContext batching)
    : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    public async Task<ProductDto?> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
        => await batching.ProductById.LoadAsync(request.Id, cancellationToken);
}