using eShop.Catalog.Application.Brands.Contracts;
using eShop.Catalog.Application.Brands.Errors;
using eShop.Catalog.Entities.Brands;
using HotChocolate.Pagination;

namespace eShop.Catalog.Application.Brands.Services;

public sealed class BrandService(
    IBrandRepository repository,
    IBrandBatchingContext batching)
{
    public async Task<Brand?> GetBrandByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
        => await batching.BrandById.LoadAsync(id, cancellationToken);

    public async Task<Brand?> GetBrandByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
        => await batching.BrandByName.LoadAsync(name, cancellationToken);

    public async Task<Page<Brand>> GetBrandsAsync(
        PagingArguments args,
        CancellationToken cancellationToken = default) 
        => await batching.Brands.LoadAsync(args, cancellationToken);

    public async Task CreateBrandAsync(Brand brand, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(brand.Name))
        {
            ArgumentException.ThrowIfNullOrEmpty(brand.Name);
        }
        
        repository.AddBrand(brand);
        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
    
    public async Task RenameBrandAsync(int id, string newName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(newName))
        {
            ArgumentException.ThrowIfNullOrEmpty(newName);
        }

        var brand = await repository.GetBrandAsync(id, cancellationToken);
        
        if (brand == null)
        {
            throw new BrandNotFoundException(id);
        }

        brand.Name = newName;
        repository.UpdateBrand(brand);
        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<bool> DeleteBrandAsync(int id, CancellationToken cancellationToken)
    {
        var brand = await repository.GetBrandAsync(id, cancellationToken);
        
        if (brand == null)
        {
            return false;
        }
        
        repository.DeleteBrand(brand);
        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}