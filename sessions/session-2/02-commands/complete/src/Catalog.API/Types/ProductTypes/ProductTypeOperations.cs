using eShop.Catalog.Application.ProductTypes.Commands;
using eShop.Catalog.Application.ProductTypes.Errors;
using eShop.Catalog.Application.ProductTypes.Services;
using eShop.Catalog.Entities.ProductTypes;
using HotChocolate.Pagination;
using HotChocolate.Types.Pagination;
using MediatR;

namespace eShop.Catalog.Types.ProductTypes;

public static class ProductTypeOperations
{
    [Query]
    [NodeResolver]
    public static async Task<ProductType?> GetProductTypeByIdAsync(
        int id,
        ProductTypeService productTypeService,
        CancellationToken ct)
        => await productTypeService.GetProductTypeByIdAsync(id, ct);

    [Query]
    public static async Task<ProductType?> GetProductTypeByNameAsync(
        string name,
        ProductTypeService productTypeService,
        CancellationToken ct)
        => await productTypeService.GetProductTypeByNameAsync(name, ct);

    [Query]
    [UsePaging]
    public static async Task<Connection<ProductType>> GetProductTypesAsync(
        PagingArguments args,
        ProductTypeService productTypeService,
        CancellationToken ct)
        => await productTypeService.GetProductTypesAsync(args, ct).ToConnectionAsync();


    [Mutation]
    public static async Task<ProductType> CreateProductType(
        string name,
        IMediator mediator,
        CancellationToken ct)
    {
        return await mediator.Send(new CreateProductTypeCommand(name), ct);
    }

    [Mutation]
    [Error<ProductTypeNotFoundException>]
    public static async Task<ProductType> RenameProductType(
        [ID<ProductType>] int id,
        string name,
        IMediator mediator,
        CancellationToken ct)
    {
        return await mediator.Send(new RenameProductTypeCommand(id, name), ct);
    }
}