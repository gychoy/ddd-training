using eShop.Catalog.Exceptions;

namespace eShop.Catalog.Entities.Brands.Exceptions;

public sealed class BrandNotFoundException(int id)
    : CatalogDomainException($"Brand not found with id {id}")
{
    public int Id { get; } = id;
}