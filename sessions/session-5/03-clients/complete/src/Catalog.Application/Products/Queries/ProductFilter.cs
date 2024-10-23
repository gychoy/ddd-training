namespace eShop.Catalog.Application.Products.Queries;

public readonly record struct ProductFilter(
    IReadOnlyList<int>? BrandIds,
    IReadOnlyList<int>? TypeIds);