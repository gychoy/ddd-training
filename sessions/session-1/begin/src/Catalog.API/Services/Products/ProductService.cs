using eShop.Catalog.Data;
using eShop.Catalog.Models;
using eShop.Catalog.Services.Errors;
using GreenDonut.Selectors;
using HotChocolate.Pagination;
using Microsoft.EntityFrameworkCore;

namespace eShop.Catalog.Services;

public sealed class ProductService(
    CatalogContext context, 
    IProductBatchingContext batchingContext)
{
    public async Task<Product?> GetProductByIdAsync(
        int id, 
        CancellationToken cancellationToken = default)
        => await batchingContext.ProductById
            .LoadAsync(id, cancellationToken);
    
    public async Task<Page<Product>> GetProductsAsync(
        PagingArguments pagingArguments,  
        ProductFilter? productFilter = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.Products.AsNoTracking();

        if (productFilter?.BrandIds is { Count: > 0 } brandIds)
        {
            query = query.Where(p => brandIds.Contains(p.BrandId));
        }
        
        if (productFilter?.TypeIds is { Count: > 0 } typeIds)
        {
            query = query.Where(p => typeIds.Contains(p.TypeId));
        }

        return await query.OrderBy(t => t.Name).ThenBy(t => t.Id).ToPageAsync(pagingArguments, cancellationToken);
    }

    public async Task<Page<Product>?> GetProductsByBrandAsync(
        int brandId,
        PagingArguments args,
        CancellationToken ct = default)
        => await batchingContext.ProductsByBrandId
            .WithPagingArguments(args)
            .LoadAsync(brandId, ct);

    public async Task<Page<Product>?> GetProductsByTypeAsync(
        int typeId,
        PagingArguments args,
        CancellationToken ct = default)
        => await batchingContext.ProductsByTypeId
            .WithPagingArguments(args)
            .LoadAsync(typeId, ct);

    public async Task CreateProductAsync(Product product, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(product.Name);
        
        if (product.RestockThreshold >= product.MaxStockThreshold)
        {
            throw new MaxStockThresholdToSmallException(product.RestockThreshold, product.MaxStockThreshold);
        }
        
        if (!await context.Brands.AnyAsync(t => t.Id == product.BrandId, cancellationToken))
        {
            throw new BrandNotFoundException(product.BrandId);
        }
        
        if (!await context.ProductTypes.AnyAsync(t => t.Id == product.TypeId, cancellationToken))
        {
            throw new ProductTypeNotFoundException(product.TypeId);
        }
        
        context.Products.Add(product);
        await context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task UpdateProductAsync(Product product, CancellationToken cancellationToken)
    {
        if (product.Id < 1)
        {
            throw new InvalidOperationException("Invalid product id.");
        }
        
        ArgumentException.ThrowIfNullOrEmpty(product.Name);
        
        if (product.RestockThreshold >= product.MaxStockThreshold)
        {
            throw new MaxStockThresholdToSmallException(product.RestockThreshold, product.MaxStockThreshold);
        }
        
        if (!await context.Brands.AnyAsync(t => t.Id == product.BrandId, cancellationToken))
        {
            throw new BrandNotFoundException(product.BrandId);
        }
        
        if (!await context.ProductTypes.AnyAsync(t => t.Id == product.TypeId, cancellationToken))
        {
            throw new ProductTypeNotFoundException(product.TypeId);
        }

        context.Products.Entry(product).State = EntityState.Modified;
        await context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<Product> RemoveStockAsync(
        int id, 
        int quantityDesired, 
        CancellationToken cancellationToken = default)
    {
        var product = await context.Products.FindAsync([id], cancellationToken: cancellationToken);
        
        if (product is null)
        {
            throw new ProductNotFoundException(id);
        }
        
        if (quantityDesired <= 0)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(quantityDesired);
        }

        if (product.AvailableStock == 0)
        {
            throw new ProductOutOfStockException(id);
        }

        if (product.AvailableStock < quantityDesired)
        {
            throw new ProductNotEnoughStockException(id, product.AvailableStock, quantityDesired);
        }

        product.AvailableStock -= quantityDesired;
        await context.SaveChangesAsync(cancellationToken);
        return product;
    }

    public async Task<Product> AddStockAsync(
        int id, 
        int quantity, 
        CancellationToken cancellationToken)
    {
        var product = await context.Products.FindAsync([id], cancellationToken: cancellationToken);
        
        if (product is null)
        {
            throw new ProductNotFoundException(id);
        }
        
        if (product.AvailableStock + quantity > product.MaxStockThreshold)
        {
            throw new ProductMaxStockThresholdReachedException(
                id,
                product.MaxStockThreshold,
                product.AvailableStock,
                quantity);
        }

        product.AvailableStock += quantity;
        product.OnReorder = false;
        await context.SaveChangesAsync(cancellationToken);
        return product;
    }
}
