using eShop.Catalog.Models;
using eShop.Catalog.Services;
using eShop.Catalog.Services.Errors;
using HotChocolate.Pagination;
using HotChocolate.Types.Pagination;

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
        ProductTypeService productTypeService,
        CancellationToken ct)
    {
        var productType = new ProductType { Name = name };
        await productTypeService.CreateProductTypeAsync(productType, ct);
        return productType;
    }

    [Mutation]
    [Error<ProductTypeNotFoundException>]
    public static async Task<ProductType> RenameProductType(
        int id,
        string name,
        ProductTypeService productTypeService,
        CancellationToken ct)
    {
        await productTypeService.RenameProductTypeAsync(id, name, ct);
        return (await productTypeService.GetProductTypeByIdAsync(id, ct))!;
    }
}