using eShop.Catalog.Exceptions;

namespace eShop.Catalog.Entities.ProductTypes.Errors;

public sealed class ProductTypeNotFoundException(int id)
    : CatalogDomainException($"Product not found with id {id}")
{
    public int Id { get; } = id;
}