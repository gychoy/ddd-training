namespace eShop.Catalog.Services.Errors;

public sealed class ProductOutOfStockException(
    int productId)
    : Exception($"Product is out of stock with id {productId}")
{
    public int ProductId { get; } = productId;
}