using System.ComponentModel.DataAnnotations;
using eShop.Catalog.Common;
using eShop.Catalog.Entities.Products.Errors;

namespace eShop.Catalog.Entities.Products;

public sealed class Product : Entity
{
    private Product()
    {
    }

    public int Id { get; private set; }

    [Required]
    public string Name { get; private set; } = default!;

    public string? Description { get; private set; }

    public decimal Price { get; private set; }

    public string? ImageFileName { get; private set; } = default!;

    public int TypeId { get; private set; }

    public int BrandId { get; private set; }

    /// <summary>
    /// Quantity in stock.
    /// </summary>
    public int AvailableStock { get; private set; }

    /// <summary>
    /// Available stock at which we should reorder.
    /// </summary>
    public int RestockThreshold { get; private set; }

    /// <summary>
    /// Maximum number of units that can be in-stock at any time
    /// (due to physicial/logistical constraints in warehouses). 
    /// </summary>
    public int MaxStockThreshold { get; private set; }

    /// <summary>
    /// True if item is on reorder
    /// </summary>
    public bool OnReorder { get; private set; }

    public void Rename(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
    }

    public void ChangePrice(decimal price)
    {
        if (price < 0)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(price);
        }

        Price = price;
    }

    public void AddStock(int quantity)
    {
        if (AvailableStock + quantity > MaxStockThreshold)
        {
            throw new ProductMaxStockThresholdReachedException(
                Id,
                MaxStockThreshold,
                AvailableStock,
                quantity);
        }

        AvailableStock += quantity;
        OnReorder = false;
    }

    public static Product Create(
        string name,
        string? description,
        decimal initialPrice,
        int brandId,
        int typeId,
        int restockThreshold,
        int maxStockThreshold)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var product = new Product
        {
            Name = name,
            Description = description,
            Price = initialPrice,
            TypeId = typeId,
            BrandId = brandId,
            RestockThreshold = restockThreshold,
            MaxStockThreshold = maxStockThreshold
        };

        if (product.RestockThreshold >= product.MaxStockThreshold)
        {
            throw new MaxStockThresholdToSmallException(
                product.RestockThreshold,
                product.MaxStockThreshold);
        }

        return product;
    }
}
