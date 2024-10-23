using eShop.Catalog.Data;
using eShop.Catalog.Models;
using HotChocolate.Pagination;
using Microsoft.EntityFrameworkCore;

namespace eShop.Catalog.Services;

public sealed class BrandService(
    CatalogContext context,
    IBrandBatchingContext batchingContext)
{
    public async Task<Brand?> GetBrandByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
        => await batchingContext.BrandById.LoadAsync(id, cancellationToken);

    public async Task<Brand?> GetBrandByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
        => await batchingContext.BrandByName.LoadAsync(name, cancellationToken);

    public async Task<Page<Brand>> GetBrandsAsync(
        PagingArguments args,
        CancellationToken cancellationToken = default) 
        => await context.Brands
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .ThenBy(t => t.Id)
            .ToPageAsync(args, cancellationToken);

    public async Task CreateBrandAsync(Brand brand, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(brand.Name))
        {
            ArgumentException.ThrowIfNullOrEmpty(brand.Name);
        }
        
        context.Brands.Add(brand);
        await context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task RenameBrandAsync(int id, string newName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(newName))
        {
            ArgumentException.ThrowIfNullOrEmpty(newName);
        }
        
        await context.Brands
            .Where(t => t.Id == id)
            .ExecuteUpdateAsync(
                t => t.SetProperty(p => p.Name, newName), 
                cancellationToken);
    }
    
    public async Task<bool> DeleteBrandAsync(int id, CancellationToken cancellationToken)
    {
        var affectedRows = await context.Brands.Where(t => t.Id == id).ExecuteDeleteAsync(cancellationToken);
        return affectedRows > 0;
    }
}