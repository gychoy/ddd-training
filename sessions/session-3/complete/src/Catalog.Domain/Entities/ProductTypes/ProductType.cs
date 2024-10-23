// ReSharper disable CollectionNeverUpdated.Global

using System.ComponentModel.DataAnnotations;

namespace eShop.Catalog.Entities.ProductTypes;

public sealed class ProductType 
{
    private ProductType()
    {
    }

    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = default!;

    public void Rename(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
    }

    public static ProductType Create(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var productType = new ProductType { Name = name };
        return productType;
    }
}
