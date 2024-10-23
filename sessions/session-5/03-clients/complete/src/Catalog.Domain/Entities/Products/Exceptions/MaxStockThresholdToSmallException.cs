using eShop.Catalog.Exceptions;

namespace eShop.Catalog.Entities.Products.Errors;

public class MaxStockThresholdToSmallException(int restockThreshold, int maxStockThreshold) 
    : CatalogDomainException
{
    public int RestockThreshold { get; } = restockThreshold;
    public int MaxStockThreshold { get; } = maxStockThreshold;
}