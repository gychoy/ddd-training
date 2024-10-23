using eShop.Catalog.Exceptions;

namespace eShop.Catalog.Entities.Products.Exceptions;

public sealed class ProductNotEnoughStockException(
    int productId,
    int availableStock,
    int quantityDesired)
    : CatalogDomainException($"Product does not have enough stock with id {productId}")
{
    public int ProductId { get; } = productId;
    public int AvailableStock { get; } = availableStock;
    public int QuantityDesired { get; } = quantityDesired;
}