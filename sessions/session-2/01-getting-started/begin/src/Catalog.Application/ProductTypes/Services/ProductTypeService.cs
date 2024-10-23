using eShop.Catalog.Application.ProductTypes.DataLoader;
using eShop.Catalog.Application.ProductTypes.Errors;
using eShop.Catalog.Entities.ProductTypes;
using HotChocolate.Pagination;

namespace eShop.Catalog.Application.ProductTypes.Services;

public sealed class ProductTypeService(
    IProductTypeRepository repository,
    IProductTypeBatchingContext batching)
{
    public async Task<ProductType?> GetProductTypeByIdAsync(
        int id, 
        CancellationToken cancellationToken = default)
        => await batching.ProductTypeById.LoadAsync(id, cancellationToken);
    
    public async Task<ProductType?> GetProductTypeByNameAsync(
        string name, 
        CancellationToken cancellationToken = default)
        => await batching.ProductTypeByName.LoadAsync(name, cancellationToken);
    
    public async Task<Page<ProductType>> GetProductTypesAsync(
        PagingArguments pagingArguments,
        CancellationToken cancellationToken = default)
        => await batching.ProductTypes.LoadAsync(pagingArguments, cancellationToken);

    public async Task CreateProductTypeAsync(ProductType type, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(type.Name))
        {
            ArgumentException.ThrowIfNullOrEmpty(type.Name);
        }
        
        repository.AddProductType(type);
        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
    
    public async Task RenameProductTypeAsync(int id, string newName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(newName))
        {
            ArgumentException.ThrowIfNullOrEmpty(newName);
        }

        var type = await repository.GetProductTypeAsync(id, cancellationToken);
        
        if(type is null) 
        {
            throw new ProductTypeNotFoundException(id);
        }

        type.Name = newName;
        repository.UpdateProductType(type);
        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<bool> DeleteProductTypeAsync(int id, CancellationToken cancellationToken)
    {
        var type = await repository.GetProductTypeAsync(id, cancellationToken);
        
        if(type is null)
        {
            return false;
        }
        
        repository.DeleteProductType(type);
        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}