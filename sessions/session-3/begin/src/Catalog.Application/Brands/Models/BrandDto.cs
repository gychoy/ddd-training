using System.ComponentModel.DataAnnotations;

namespace eShop.Catalog.Application.Brands;

public class BrandDto
{
    public required int Id { get; set; }

    [Required]
    public required string Name { get; set; }
}