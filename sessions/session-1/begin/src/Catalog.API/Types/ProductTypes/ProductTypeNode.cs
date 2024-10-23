using eShop.Catalog.Models;
using eShop.Catalog.Services;
using HotChocolate.Pagination;
using HotChocolate.Types.Pagination;

namespace eShop.Catalog.Types.ProductTypes;

[ObjectType<ProductType>]
public static partial class ProductTypeNode
{
    [UsePaging(ConnectionName = "ProductTypeProducts")]
    public static async Task<Connection<Product>> GetProductsAsync(
        [Parent(nameof(ProductType.Id))] ProductType productType,
        PagingArguments pagingArguments,
        ProductService productService,
        CancellationToken cancellationToken)
        => await productService
            .GetProductsByTypeAsync(
                productType.Id, 
                pagingArguments, 
                cancellationToken)
            .ToConnectionAsync();
}