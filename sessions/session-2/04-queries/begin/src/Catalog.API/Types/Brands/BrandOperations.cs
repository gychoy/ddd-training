using eShop.Catalog.Application.Brands.Commands;
using eShop.Catalog.Application.Brands.Errors;
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
    public static async Task<Brand?> GetBrandByIdAsync(
        int id,
        BrandService brandService,
        CancellationToken ct)
        => await brandService.GetBrandByIdAsync(id, ct);

    [Query]
    public static async Task<Brand?> GetBrandByNameAsync(
        string name,
        BrandService brandService,
        CancellationToken ct)
        => await brandService.GetBrandByNameAsync(name, ct);

    [Query]
    [UsePaging]
    public static async Task<Connection<Brand>> GetBrandsAsync(
        PagingArguments args,
        BrandService brandService,
        CancellationToken ct)
        => await brandService.GetBrandsAsync(args, ct).ToConnectionAsync();


    [Mutation]
    public static async Task<Brand> CreateBrand(
        string name,
        IMediator mediator,
        CancellationToken ct)
    {
        return await mediator.Send(new CreateBrandCommand(name), ct);
    }

    [Mutation]
    [Error<BrandNotFoundException>]
    public static async Task<Brand> RenameBrand(
        [ID<Brand>] int id,
        string name,
        IMediator mediator,
        CancellationToken ct)
    {
        return await mediator.Send(new RenameBrandCommand(id, name), ct);
    }
}