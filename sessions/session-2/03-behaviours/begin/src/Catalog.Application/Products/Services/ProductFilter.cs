namespace eShop.Catalog.Application.Products.Services;

public readonly record struct ProductFilter(
    IReadOnlyList<int>? BrandIds,
    IReadOnlyList<int>? TypeIds);