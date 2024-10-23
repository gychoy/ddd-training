using eShop.Catalog.Common;
using eShop.Catalog.Entities.ProductTypes;
using Microsoft.EntityFrameworkCore;

namespace eShop.Catalog.Infrastructure.Repositories;

public sealed class ProductTypeRepository(
    CatalogContext context,
    IUnitOfWork unitOfWork)
    : IProductTypeRepository
{
    public IUnitOfWork UnitOfWork => unitOfWork;

    public async ValueTask<ProductType?> GetProductTypeAsync(
        int id,
        CancellationToken cancellationToken = default)
        => await context.ProductTypes.FindAsync([id], cancellationToken);

    public void AddProductType(ProductType productType)
        => context.ProductTypes.Add(productType);

    public void UpdateProductType(ProductType productType)
        => context.Entry(productType).State = EntityState.Modified;

    public void DeleteProductType(ProductType productType)
        => context.Entry(productType).State = EntityState.Deleted;
}
