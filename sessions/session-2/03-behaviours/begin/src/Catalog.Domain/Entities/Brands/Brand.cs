// ReSharper disable CollectionNeverUpdated.Global

using System.ComponentModel.DataAnnotations;
using eShop.Catalog.Entities.Products;

namespace eShop.Catalog.Entities.Brands;

public sealed class Brand
{
    public int Id { get; set; }

    [Required]
    public required string Name { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
