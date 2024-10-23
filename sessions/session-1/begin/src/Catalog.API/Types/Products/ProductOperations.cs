using eShop.Catalog.Models;
using eShop.Catalog.Services;
using eShop.Catalog.Services.Errors;
using HotChocolate.Pagination;
using HotChocolate.Types.Pagination;

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
        ProductsFilterInput where,
        ProductService productService,
        CancellationToken ct)
        => await productService.GetProductsAsync(args, where.ToFilter(), ct).ToConnectionAsync();

    [Mutation]
    public static async Task<Product> CreateProductAsync(
        CreateProductInput input,
        ProductService productService,
        CancellationToken ct)
    {
        var product = new Product
        {
            Name = input.Name,
            Description = input.Description,
            Price = input.InitialPrice,
            BrandId = input.BrandId,
            TypeId = input.TypeId,
            RestockThreshold = input.RestockThreshold,
            MaxStockThreshold = input.MaxStockThreshold
        };
        
        await productService.CreateProductAsync(product, ct);
        
        return product;
    }
    
    [Mutation]
    [Error<ProductNotFoundException>]
    public static async Task<Product> RenameProductAsync(
        int id,
        string newName,
        ProductService productService,
        CancellationToken ct)
    {
        var product = await productService.GetProductByIdAsync(id, ct);
        
        if (product == null)
        {
            throw new ProductNotFoundException(id);
        }

        product.Name = newName;
        await productService.UpdateProductAsync(product, ct);
        
        return product;
    }

    [Mutation]
    [Error<ProductNotFoundException>]
    public static async Task<Product> ChangeProductPriceAsync(
        int id,
        decimal newPrice,
        ProductService productService,
        CancellationToken ct)
    {
        var product = await productService.GetProductByIdAsync(id, ct);
        
        if (product == null)
        {
            throw new ProductNotFoundException(id);
        }

        product.Price = newPrice;
        await productService.UpdateProductAsync(product, ct);

        return product;
    }
    
    [Mutation]
    [Error<ProductOutOfStockException>]
    [Error<ProductNotEnoughStockException>]
    public static async Task<Product> AddProductStockAsync(
        int id,
        int quantity,
        ProductService productService,
        CancellationToken ct) 
        => await productService.AddStockAsync(id, quantity, ct);

    [Mutation]
    [Error<ProductOutOfStockException>]
    [Error<ProductNotEnoughStockException>]
    public static async Task<Product> RemoveProductStockAsync(
        int id,
        int quantityDesired,
        ProductService productService,
        CancellationToken ct) 
        => await productService.AddStockAsync(id, quantityDesired, ct);
}