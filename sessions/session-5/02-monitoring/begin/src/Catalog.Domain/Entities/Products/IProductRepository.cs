using eShop.Catalog.Common;

namespace eShop.Catalog.Entities.Products;

public interface IProductRepository : IRepository
{
    ValueTask<Product?> GetProductAsync(
        int id,
        CancellationToken cancellationToken = default);

    void AddProduct(Product product);

    void UpdateProduct(Product product);
    
    void DeleteProduct(Product product);
}
