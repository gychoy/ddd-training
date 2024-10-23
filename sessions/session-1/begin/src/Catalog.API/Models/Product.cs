using System.ComponentModel.DataAnnotations;

namespace eShop.Catalog.Models;

public sealed class Product
{
    public int Id { get; set; }

    [Required]
    public required string Name { get; set; }

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public string? ImageFileName { get; set; }

    public int TypeId { get; set; }

    public ProductType? Type { get; set; }

    public int BrandId { get; set; }

    public Brand? Brand { get; set; }

    /// <summary>
    /// Quantity in stock.
    /// </summary>
    public int AvailableStock { get; set; }

    /// <summary>
    /// Available stock at which we should reorder.
    /// </summary>
    public int RestockThreshold { get; set; }
    
    /// <summary>
    /// Maximum number of units that can be in-stock at any time
    /// (due to physicial/logistical constraints in warehouses). 
    /// </summary>
    public int MaxStockThreshold { get; set; }

    /// <summary>
    /// True if item is on reorder
    /// </summary>
    public bool OnReorder { get; set; }
}
