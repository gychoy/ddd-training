using eShop.Catalog.Entities.ProductTypes;

namespace eShop.Catalog.Application.ProductTypes.Models;

public static class ProductTypeExtensions
{
    public static ProductTypeDto ToReadModel(this ProductType productType)
    {
        return new ProductTypeDto
        {
            Id = productType.Id,
            Name = productType.Name
        };
    }
}