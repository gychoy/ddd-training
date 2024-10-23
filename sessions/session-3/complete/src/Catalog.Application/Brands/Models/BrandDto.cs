using System.ComponentModel.DataAnnotations;

namespace eShop.Catalog.Application.Brands.Models;

public class BrandDto
{
    public required int Id { get; set; }

    [Required]
    public required string Name { get; set; }
}