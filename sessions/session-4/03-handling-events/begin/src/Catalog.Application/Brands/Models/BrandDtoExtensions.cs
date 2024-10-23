using eShop.Catalog.Entities.Brands;

namespace eShop.Catalog.Application.Brands;

public static class BrandDtoExtensions
{
    public static BrandDto ToReadModel(this Brand brand)
        => new()
        {
            Id = brand.Id,
            Name = brand.Name
        };
}