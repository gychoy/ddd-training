using eShop.Catalog.Application.Products.Commands;
using eShop.Catalog.Application.Products.Errors;
using eShop.Catalog.Application.Products.Models;
using eShop.Catalog.Application.Products.Queries;
using eShop.Catalog.Entities.Products;
using HotChocolate.Pagination;
using HotChocolate.Types.Pagination;
using MediatR;

namespace eShop.Catalog.Types.Products;

public static class ProductOperations
{
    [Query]
    [NodeResolver]
    public static async Task<ProductDto?> GetProductByIdAsync(
        int id,
        IMediator mediator,
        CancellationToken ct)
        => await mediator.Send(new GetProductByIdQuery(id), ct);

    [Query]
    [UsePaging]
    public static async Task<Connection<ProductDto>> GetProductsAsync(
        PagingArguments args,
        ProductsFilterInput? where,
        IMediator mediator,
        CancellationToken ct)
        => await mediator
            .Send(new GetProductsQuery(args, where?.ToFilter()), ct)
            .ToConnectionAsync();

    [Mutation]
    public static async Task<ProductDto> CreateProductAsync(
        IMediator mediator,
        CreateProductCommand input,
        CancellationToken ct) =>
        await mediator.Send(input, ct);

    [Mutation]
    [Error<ProductNotFoundException>]
    public static async Task<ProductDto> RenameProductAsync(
        [ID<Product>] int id,
        string newName,
        IMediator mediator,
        CancellationToken ct) =>
        await mediator.Send(new RenameProductCommand(id, newName), ct);
    
    [Mutation]
    [Error<ProductNotFoundException>]
    public static async Task<ProductDto> ChangeProductDescriptionAsync(
        [ID<Product>] int id,
        string newDescription,
        IMediator mediator,
        CancellationToken ct)
        => await mediator.Send(new ChangeProductDescriptionCommand(id, newDescription), ct);

    [Mutation]
    [Error<ProductNotFoundException>]
    public static async Task<ProductDto> ChangeProductPriceAsync(
        [ID<Product>] int id,
        decimal newPrice,
        IMediator mediator,
        CancellationToken ct) =>
        await mediator.Send(new ChangeProductPriceCommand(id, newPrice), ct);

    [Mutation]
    [Error<ProductNotFoundException>]
    [Error<ProductMaxStockThresholdReachedException>]
    public static async Task<ProductDto> AddProductStockAsync(
        [ID<Product>] int id,
        int quantity,
        IMediator mediator,
        CancellationToken ct)
        => await mediator.Send(new AddProductStockCommand(id, quantity), ct);
    
    [Mutation]
    [Error<ProductNotFoundException>]
    [Error<ProductOutOfStockException>]
    [Error<ProductNotEnoughStockException>]
    public static async Task<ProductDto> RemoveProductStockAsync(
        [ID<Product>] int id,
        int quantityDesired,
        IMediator mediator,
        CancellationToken ct) 
        => await mediator.Send(new RemoveProductStockCommand(id, quantityDesired), ct);
}
