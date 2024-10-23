namespace eShop.Catalog.Application.Products.Errors;

public sealed class ProductMaxStockThresholdReachedException(
    int productId,
    int maxStockThreshold,
    int currentAvailableStock,
    int quantityAdded)
    : Exception($"Maximum stock threshold reached for product id {productId}")
{
    public int ProductId { get; } = productId;
    public int MaxStockThreshold { get; } = maxStockThreshold;
    public int CurrentAvailableStock { get; } = currentAvailableStock;
    public int QuantityAdded { get; } = quantityAdded;
}