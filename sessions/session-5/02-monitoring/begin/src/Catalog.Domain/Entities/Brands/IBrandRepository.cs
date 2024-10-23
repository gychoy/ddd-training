using eShop.Catalog.Common;

namespace eShop.Catalog.Entities.Brands;

public interface IBrandRepository : IRepository
{
    ValueTask<Brand?> GetBrandAsync(
        int id,
        CancellationToken cancellationToken = default);

    void AddBrand(Brand brand);

    void UpdateBrand(Brand brand);
    
    void DeleteBrand(Brand brand);
}
