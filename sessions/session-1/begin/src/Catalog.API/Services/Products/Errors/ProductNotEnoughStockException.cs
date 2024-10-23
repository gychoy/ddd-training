namespace eShop.Catalog.Services.Errors;

public sealed class ProductNotEnoughStockException(
    int productId,
    int availableStock,
    int quantityDesired)
    : Exception($"Product does not have enough stock with id {productId}")
{
    public int ProductId { get; } = productId;
    public int AvailableStock { get; } = availableStock;
    public int QuantityDesired { get; } = quantityDesired;
}