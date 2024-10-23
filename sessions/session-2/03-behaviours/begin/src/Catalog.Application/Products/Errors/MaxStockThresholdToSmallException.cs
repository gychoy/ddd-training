namespace eShop.Catalog.Application.Products.Errors;

public class MaxStockThresholdToSmallException(int restockThreshold, int maxStockThreshold) : Exception
{
    public int RestockThreshold { get; } = restockThreshold;
    public int MaxStockThreshold { get; } = maxStockThreshold;
}