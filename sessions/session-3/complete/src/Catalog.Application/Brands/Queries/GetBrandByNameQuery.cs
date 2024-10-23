using eShop.Catalog.Application.Brands.Contracts;
using eShop.Catalog.Application.Brands.Models;
using eShop.Catalog.Application.Common.Contracts;
using MediatR;

namespace eShop.Catalog.Application.Brands.Queries;

public sealed record GetBrandByNameQuery(string Name) 
    : IQuery<BrandDto?>;

public sealed class GetBrandByNameQueryHandler(IBrandBatchingContext batching)
    : IRequestHandler<GetBrandByNameQuery, BrandDto?>
{
    public async Task<BrandDto?> Handle(
        GetBrandByNameQuery request,
        CancellationToken cancellationToken)
        => await batching.BrandByName.LoadAsync(request.Name, cancellationToken);
}