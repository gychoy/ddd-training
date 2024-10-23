using eShop.Catalog.Application.ProductTypes.Commands;
using eShop.Catalog.Application.ProductTypes.Models;
using eShop.Catalog.Application.ProductTypes.Queries;
using eShop.Catalog.Entities.ProductTypes;
using eShop.Catalog.Entities.ProductTypes.Errors;
using HotChocolate.Pagination;
using HotChocolate.Types.Pagination;
using MediatR;

namespace eShop.Catalog.Types.ProductTypes;

public static class ProductTypeOperations
{
    [Query]
    [NodeResolver]
    public static async Task<ProductTypeDto?> GetProductTypeByIdAsync(
        int id,
        IMediator mediator,
        CancellationToken ct)
        => await mediator.Send(new GetProductTypeByIdQuery(id), ct);

    [Query]
    public static async Task<ProductTypeDto?> GetProductTypeByNameAsync(
        string name,
        IMediator mediator,
        CancellationToken ct)
        => await mediator.Send(new GetProductTypeByNameQuery(name), ct);

    [Query]
    [UsePaging]
    public static async Task<Connection<ProductTypeDto>> GetProductTypesAsync(
        PagingArguments args,
        IMediator mediator,
        CancellationToken ct)
        => await mediator
            .Send(new GetProductTypesQuery(args), ct)
            .ToConnectionAsync();

    [Mutation]
    public static async Task<ProductTypeDto> CreateProductType(
        string name,
        IMediator mediator,
        CancellationToken ct)
    {
        return await mediator.Send(new CreateProductTypeCommand(name), ct);
    }

    [Mutation]
    [Error<ProductTypeNotFoundException>]
    public static async Task<ProductTypeDto> RenameProductType(
        [ID<ProductType>] int id,
        string name,
        IMediator mediator,
        CancellationToken ct)
    {
        return await mediator.Send(new RenameProductTypeCommand(id, name), ct);
    }
}