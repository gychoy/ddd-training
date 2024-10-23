using eShop.Catalog.Services;

namespace eShop.Catalog.Types.Products;

public readonly record struct ProductsFilterInput(
    ProductsBrandIdFilterInput? BrandId,
    ProductsTypeIdFilterInput? TypeId)
{
    public ProductFilter ToFilter() => new(BrandId?.In, TypeId?.In);
}

public readonly record struct ProductsBrandIdFilterInput(int[]? In);

public readonly record struct ProductsTypeIdFilterInput(int[]? In);