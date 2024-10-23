using eShop.Catalog.Entities.Brands;
using eShop.Catalog.Entities.ProductTypes;

namespace eShop.Catalog.Types.Products;

public sealed record CreateProductInput(
    string Name,
    string? Description,
    decimal InitialPrice,
    [property: ID<Brand>] int BrandId,
    [property: ID<ProductType>] int TypeId,
    int RestockThreshold,
    int MaxStockThreshold);
    