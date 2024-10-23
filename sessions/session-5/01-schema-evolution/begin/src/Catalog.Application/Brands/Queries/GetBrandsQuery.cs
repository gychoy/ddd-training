using eShop.Catalog.Application.Brands.Contracts;
using eShop.Catalog.Application.Common.Contracts;
using eShop.Catalog.Entities.Brands;
using HotChocolate.Pagination;
using MediatR;

namespace eShop.Catalog.Application.Brands.Queries;

public sealed record GetBrandsQuery(PagingArguments PagingArgs) : IQuery<Page<BrandDto>>;

public sealed class GetBrandsQueryHandler(IBrandBatchingContext batching)
    : IRequestHandler<GetBrandsQuery, Page<BrandDto>>
{
    public async Task<Page<BrandDto>> Handle(
        GetBrandsQuery request,
        CancellationToken cancellationToken)
        => await batching.Brands.LoadAsync(request.PagingArgs, cancellationToken);
}