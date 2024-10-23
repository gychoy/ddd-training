using eShop.Catalog.Exceptions;

namespace eShop.Catalog.Entities.Products.Errors;

public sealed class ProductOutOfStockException(
    int productId)
    : CatalogDomainException($"Product is out of stock with id {productId}")
{
    public int ProductId { get; } = productId;
}