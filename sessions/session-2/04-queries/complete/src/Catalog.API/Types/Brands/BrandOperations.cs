using eShop.Catalog.Application.Brands;
using eShop.Catalog.Application.Brands.Commands;
using eShop.Catalog.Application.Brands.Errors;
using eShop.Catalog.Application.Brands.Queries;
using eShop.Catalog.Application.Brands.Services;
using eShop.Catalog.Entities.Brands;
using HotChocolate.Pagination;
using HotChocolate.Types.Pagination;
using MediatR;

namespace eShop.Catalog.Types.Brands;

public static class BrandOperations
{
    [Query]
    [NodeResolver]
    public static async Task<BrandDto?> GetBrandByIdAsync(
        int id,
        IMediator mediator,
        CancellationToken ct)
        => await mediator.Send(new GetBrandByIdQuery(id), ct);

    [Query]
    public static async Task<BrandDto?> GetBrandByNameAsync(
        string name,
        IMediator mediator,
        CancellationToken ct)
        => await mediator.Send(new GetBrandByNameQuery(name), ct);

    [Query]
    [UsePaging]
    public static async Task<Connection<BrandDto>> GetBrandsAsync(
        PagingArguments args,
        IMediator mediator,
        CancellationToken ct)
        => await mediator
            .Send(new GetBrandsQuery(args), ct)
            .ToConnectionAsync();


    [Mutation]
    public static async Task<BrandDto> CreateBrand(
        string name,
        IMediator mediator,
        CancellationToken ct)
    {
        return await mediator.Send(new CreateBrandCommand(name), ct);
    }

    [Mutation]
    [Error<BrandNotFoundException>]
    public static async Task<BrandDto> RenameBrand(
        [ID<Brand>] int id,
        string name,
        IMediator mediator,
        CancellationToken ct)
    {
        return await mediator.Send(new RenameBrandCommand(id, name), ct);
    }
}