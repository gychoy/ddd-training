using eShop.Catalog.Application.Products.Contracts;
using eShop.Catalog.Entities.Products;
using GreenDonut.Selectors;
using HotChocolate.Pagination;

namespace eShop.Catalog.Application.Products.Services;

public sealed class ProductService(IProductBatchingContext batching)
{
    public async Task<Product?> GetProductByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
        => await batching.ProductById
            .LoadAsync(id, cancellationToken);

    public async Task<Page<Product>> GetProductsAsync(
        PagingArguments pagingArgs,
        ProductFilter? productFilter = null,
        CancellationToken cancellationToken = default)
        => await batching.Products
            .LoadAsync(pagingArgs, productFilter, cancellationToken);

    public async Task<Page<Product>?> GetProductsByBrandAsync(
        int brandId,
        PagingArguments args,
        CancellationToken ct = default)
        => await batching.ProductsByBrand
            .WithPagingArguments(args)
            .LoadAsync(brandId, ct);

    public async Task<Page<Product>?> GetProductsByTypeAsync(
        int typeId,
        PagingArguments args,
        CancellationToken ct = default)
        => await batching.ProductsByType
            .WithPagingArguments(args)
            .LoadAsync(typeId, ct);
}