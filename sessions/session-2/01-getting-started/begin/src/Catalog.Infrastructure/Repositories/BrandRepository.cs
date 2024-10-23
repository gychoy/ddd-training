using eShop.Catalog.Common;
using eShop.Catalog.Entities.Brands;
using Microsoft.EntityFrameworkCore;

namespace eShop.Catalog.Infrastructure.Repositories;

public sealed class BrandRepository(
    CatalogContext context,
    IUnitOfWork unitOfWork)
    : IBrandRepository
{
    public IUnitOfWork UnitOfWork => unitOfWork;

    public async ValueTask<Brand?> GetBrandAsync(
        int id,
        CancellationToken cancellationToken = default)
        => await context.Brands.FindAsync([id], cancellationToken);

    public void AddBrand(Brand brand)
        => context.Brands.Add(brand);

    public void UpdateBrand(Brand brand)
        => context.Entry(brand).State = EntityState.Modified;

    public void DeleteBrand(Brand brand)
        => context.Entry(brand).State = EntityState.Deleted;
}

