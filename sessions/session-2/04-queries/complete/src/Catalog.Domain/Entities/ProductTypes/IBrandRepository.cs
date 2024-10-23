using eShop.Catalog.Common;

namespace eShop.Catalog.Entities.ProductTypes;

public interface IProductTypeRepository : IRepository
{
    ValueTask<ProductType?> GetProductTypeAsync(
        int id,
        CancellationToken cancellationToken = default);

    void AddProductType(ProductType productType);

    void UpdateProductType(ProductType productType);
    
    void DeleteProductType(ProductType productType);
}
