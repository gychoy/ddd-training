using eShop.Catalog.Application.Brands.Contracts;
using eShop.Catalog.Application.Brands.Models;
using eShop.Catalog.Application.Common.Contracts;
using MediatR;

namespace eShop.Catalog.Application.Brands.Queries;

public sealed record GetBrandByIdQuery(int Id) 
    : IQuery<BrandDto?>;

public sealed class GetBrandByIdQueryHandler(IBrandBatchingContext batching)
    : IRequestHandler<GetBrandByIdQuery, BrandDto?>
{
    public async Task<BrandDto?> Handle(
        GetBrandByIdQuery request,
        CancellationToken cancellationToken)
        => await batching.BrandById.LoadAsync(request.Id, cancellationToken);
}