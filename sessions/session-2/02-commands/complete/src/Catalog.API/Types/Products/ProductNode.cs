using eShop.Catalog.Application.Brands.Services;
using eShop.Catalog.Application.ProductTypes.Services;
using eShop.Catalog.Entities.Brands;
using eShop.Catalog.Entities.Products;
using eShop.Catalog.Entities.ProductTypes;

namespace eShop.Catalog.Types.Products;

[ObjectType<Product>]
public static partial class ProductNode
{
    [BindMember(nameof(product.BrandId))]
    public static async Task<Brand?> GetBrandAsync(
        [Parent] Product product,
        BrandService brandService,
        CancellationToken ct)
        => await brandService.GetBrandByIdAsync(product.BrandId, ct);
    
    [BindMember(nameof(product.TypeId))]
    public static async Task<ProductType?> GetTypeAsync(
        [Parent] Product product,
        ProductTypeService productTypeService,
        CancellationToken ct)
        => await productTypeService.GetProductTypeByIdAsync(product.TypeId, ct);
}
