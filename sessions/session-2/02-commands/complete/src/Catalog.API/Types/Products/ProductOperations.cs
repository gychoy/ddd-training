using eShop.Catalog.Application.Products.Commands;
using eShop.Catalog.Application.Products.Errors;
using eShop.Catalog.Application.Products.Services;
using eShop.Catalog.Entities.Products;
using HotChocolate.Pagination;
using HotChocolate.Types.Pagination;
using MediatR;

namespace eShop.Catalog.Types.Products;

public static class ProductOperations
{
    [Query]
    [NodeResolver]
    public static async Task<Product?> GetProductByIdAsync(
        int id,
        ProductService productService,
        CancellationToken ct)
        => await productService.GetProductByIdAsync(id, ct);

    [Query]
    [UsePaging]
    public static async Task<Connection<Product>> GetProductsAsync(
        PagingArguments args,
        ProductsFilterInput? where,
        ProductService productService,
        CancellationToken ct)
        => await productService.GetProductsAsync(args, where?.ToFilter(), ct).ToConnectionAsync();

    [Mutation]
    public static async Task<Product> CreateProductAsync(
        IMediator mediator,
        CreateProductCommand input,
        CancellationToken ct)
    {
        return await mediator.Send(input, ct);
    }

    [Mutation]
    [Error<ProductNotFoundException>]
    public static async Task<Product> RenameProductAsync(
        [ID<Product>] int id,
        string newName,
        IMediator mediator,
        CancellationToken ct)
    {
        return await mediator.Send(new RenameProductCommand(id, newName), ct);
    }

    [Mutation]
    [Error<ProductNotFoundException>]
    public static async Task<Product> ChangeProductPriceAsync(
        [ID<Product>] int id,
        decimal newPrice,
        IMediator mediator,
        CancellationToken ct)
    {
        return await mediator.Send(new ChangeProductPriceCommand(id, newPrice), ct);
    }

    [Mutation]
    [Error<ProductOutOfStockException>]
    [Error<ProductNotEnoughStockException>]
    public static async Task<Product> AddProductStockAsync(
        [ID<Product>] int id,
        int quantity,
        IMediator mediator,
        CancellationToken ct)
        => await mediator.Send(new AddProductStockCommand(id, quantity), ct);
}