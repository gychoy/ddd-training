using eShop.Catalog.Models;
using eShop.Catalog.Services;
using HotChocolate.Pagination;
using HotChocolate.Types.Pagination;

namespace eShop.Catalog.Types.Brands;

[ObjectType<Brand>]
public static partial class BrandNode
{
    [UsePaging(ConnectionName = "BrandProducts")]
    public static async Task<Connection<Product>> GetProductsAsync(
        [Parent(nameof(Brand.Id))] Brand brand,
        PagingArguments pagingArguments,
        ProductService productService,
        CancellationToken cancellationToken)
        => await productService.GetProductsByBrandAsync(
            brand.Id, 
            pagingArguments, 
            cancellationToken).ToConnectionAsync();
}