using eShop.Catalog.Entities.Products;

namespace eShop.Catalog.Application.Products.Models;

public static class ProductDtoExtensions
{
    public static ProductDto ToReadModel(this Product product)
        => new()
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageFileName = product.ImageFileName,
            TypeId = product.TypeId,
            BrandId = product.BrandId,
            AvailableStock = product.AvailableStock,
            RestockThreshold = product.RestockThreshold,
            MaxStockThreshold = product.MaxStockThreshold,
            OnReorder = product.OnReorder
        };
}