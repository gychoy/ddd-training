namespace eShop.Catalog.Application.Products.Models;

public sealed class ProductDto
{
    public required int Id { get; set; }

    public required string Name { get; set; }

    public required string? Description { get; set; }

    public required decimal Price { get; set; }

    public required string? ImageFileName { get; set; }

    public required int TypeId { get; set; }

    public required int BrandId { get; set; }

    /// <summary>
    /// Quantity in stock.
    /// </summary>
    public required int AvailableStock { get; set; }

    /// <summary>
    /// Available stock at which we should reorder.
    /// </summary>
    public required int RestockThreshold { get; set; }

    /// <summary>
    /// Maximum number of units that can be in-stock at any time
    /// (due to physicial/logistical constraints in warehouses). 
    /// </summary>
    public required int MaxStockThreshold { get; set; }

    /// <summary>
    /// True if item is on reorder
    /// </summary>
    public required bool OnReorder { get; set; }
}