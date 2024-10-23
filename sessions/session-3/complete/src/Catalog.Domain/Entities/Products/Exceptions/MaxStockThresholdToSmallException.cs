using eShop.Catalog.Exceptions;

namespace eShop.Catalog.Entities.Products.Exceptions;

public class MaxStockThresholdToSmallException(int restockThreshold, int maxStockThreshold) 
    : CatalogDomainException
{
    public int RestockThreshold { get; } = restockThreshold;
    public int MaxStockThreshold { get; } = maxStockThreshold;
}