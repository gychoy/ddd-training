// ReSharper disable CollectionNeverUpdated.Global

using System.ComponentModel.DataAnnotations;
using eShop.Catalog.Common;
using eShop.Catalog.Entities.Products;
using eShop.Catalog.Events;

namespace eShop.Catalog.Entities.ProductTypes;

public sealed class ProductType : Entity
{
    private ProductType()
    {
    }

    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = default!;
    
    public int TotalProducts { get; private set; }
    
    public void IncreaseTotalProducts()
    {
        TotalProducts++;
    }

    public void Rename(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
        Events.Add(new ProductTypeRenamedEvent(this, name));
    }

    public static ProductType Create(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var productType = new ProductType { Name = name };
        productType.Events.Add(new ProductTypeCreatedEvent(productType));
        return productType;
    }
}
