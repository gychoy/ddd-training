namespace eShop.Catalog.Application.ProductTypes.Errors;

public sealed class ProductTypeNotFoundException(int id)
    : Exception($"Product not found with id {id}")
{
    public int Id { get; } = id;
}
