using eShop.Catalog.Application.Products.Services;
using eShop.Catalog.Entities.Products;
using eShop.Catalog.Entities.ProductTypes;
using HotChocolate.Pagination;
using HotChocolate.Types.Pagination;

namespace eShop.Catalog.Types.ProductTypes;

[ObjectType<ProductType>]
public static partial class ProductTypeNode
{
    [UsePaging(ConnectionName = "ProductTypeProducts")]
    public static async Task<Connection<Product>> GetProductsAsync(
        [Parent] ProductType type,
        PagingArguments pagingArgs,
        ProductService productService,
        CancellationToken ct)
        => await productService.GetProductsByTypeAsync(type.Id, pagingArgs, ct).ToConnectionAsync();
}
