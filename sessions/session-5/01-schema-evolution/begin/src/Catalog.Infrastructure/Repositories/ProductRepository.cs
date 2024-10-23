using eShop.Catalog.Common;
using eShop.Catalog.Entities.Products;
using Microsoft.EntityFrameworkCore;

namespace eShop.Catalog.Infrastructure.Repositories;

public sealed class ProductRepository(
    CatalogContext context,
    IUnitOfWork unitOfWork)
    : IProductRepository
{
    public IUnitOfWork UnitOfWork => unitOfWork;

    public async ValueTask<Product?> GetProductAsync(
        int id,
        CancellationToken cancellationToken = default)
        => await context.Products.FindAsync([id], cancellationToken);

    public void AddProduct(Product product)
        => context.Products.Add(product);

    public void UpdateProduct(Product product)
        => context.Entry(product).State = EntityState.Modified;

    public void DeleteProduct(Product product)
        => context.Entry(product).State = EntityState.Deleted;
}
