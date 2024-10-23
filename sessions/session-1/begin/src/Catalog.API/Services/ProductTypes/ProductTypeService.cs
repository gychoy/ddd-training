using System.Linq.Expressions;
using eShop.Catalog.Data;
using eShop.Catalog.Models;
using HotChocolate.Pagination;
using Microsoft.EntityFrameworkCore;

namespace eShop.Catalog.Services;

public sealed class ProductTypeService(
    CatalogContext context,
    IProductTypeBatchingContext batchingContext)
{
    public async Task<ProductType?> GetProductTypeByIdAsync(
        int id, 
        CancellationToken cancellationToken = default)
        => await batchingContext.ProductTypeById.LoadAsync(id, cancellationToken);
    
    public async Task<ProductType?> GetProductTypeByNameAsync(
        string name, 
        CancellationToken cancellationToken = default)
        => await batchingContext.ProductTypeByName.LoadAsync(name, cancellationToken);
    
    public async Task<Page<ProductType>> GetProductTypesAsync(
        PagingArguments pagingArguments,
        CancellationToken cancellationToken = default)
    {
        return await context.ProductTypes
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .ThenBy(t => t.Id)
            .ToPageAsync(pagingArguments, cancellationToken);
    }

    public async Task CreateProductTypeAsync(ProductType type, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(type.Name))
        {
            ArgumentException.ThrowIfNullOrEmpty(type.Name);
        }
        
        context.ProductTypes.Add(type);
        await context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task RenameProductTypeAsync(int id, string newName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(newName))
        {
            ArgumentException.ThrowIfNullOrEmpty(newName);
        }

        await context.ProductTypes
            .Where(t => t.Id == id)
            .ExecuteUpdateAsync(t => t.SetProperty(p => p.Name, newName), cancellationToken);
    }
    
    public async Task<bool> DeleteProductTypeAsync(int id, CancellationToken cancellationToken)
    {
        var affectedRows = await context.ProductTypes.Where(t => t.Id == id).ExecuteDeleteAsync(cancellationToken);
        return affectedRows > 0;
    }
}