using eShop.Catalog.Application.Products.Contracts;
using eShop.Catalog.Application.Products.Errors;
using eShop.Catalog.Entities.Products;
using GreenDonut.Selectors;
using HotChocolate.Pagination;

namespace eShop.Catalog.Application.Products.Services;

public sealed class ProductService(
    IProductRepository repository, 
    IProductBatchingContext batching)
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

    public async Task CreateProductAsync(Product product, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(product.Name);
        
        if (product.RestockThreshold >= product.MaxStockThreshold)
        {
            throw new MaxStockThresholdToSmallException(product.RestockThreshold, product.MaxStockThreshold);
        }
        
        repository.AddProduct(product);
        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);
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
        
        repository.UpdateProduct(product);
        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<bool> DeleteProductAsync(int id, CancellationToken cancellationToken)
    {
        var product = await repository.GetProductAsync(id, cancellationToken);
        
        if (product is null)
        {
            return false;
        }
        
        repository.DeleteProduct(product);
        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
    
    public async Task<Product> RemoveStockAsync(
        int id, 
        int quantityDesired, 
        CancellationToken cancellationToken = default)
    {
        var product = await repository.GetProductAsync(id, cancellationToken);
        
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
        repository.UpdateProduct(product);
        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);
        return product;
    }

    public async Task<Product> AddStockAsync(
        int id, 
        int quantity, 
        CancellationToken cancellationToken)
    {
        var product = await repository.GetProductAsync(id, cancellationToken);
        
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
        repository.UpdateProduct(product);
        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);
        return product;
    }
}
