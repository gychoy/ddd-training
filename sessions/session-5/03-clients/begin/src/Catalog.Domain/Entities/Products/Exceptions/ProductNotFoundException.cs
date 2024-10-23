using eShop.Catalog.Exceptions;

namespace eShop.Catalog.Entities.Products.Errors;

public sealed class ProductNotFoundException(int id)
    : CatalogDomainException($"Product not found with id {id}")
{
    public int Id { get; } = id;
}
