using eShop.Catalog.Application.Products.Services;
using eShop.Catalog.Entities.Brands;
using eShop.Catalog.Entities.Products;
using HotChocolate.Execution.Processing;
using HotChocolate.Pagination;
using HotChocolate.Types.Pagination;

namespace eShop.Catalog.Types.Brands;

[ObjectType<Brand>]
public static partial class BrandNode
{
    [UsePaging(ConnectionName = "BrandProducts")]
    public static async Task<Connection<Product>> GetProductsAsync(
        [Parent] Brand brand,
        PagingArguments pagingArgs,
        ProductService productService,
        CancellationToken ct)
        => await productService.GetProductsByBrandAsync(brand.Id, pagingArgs, ct).ToConnectionAsync();
}
