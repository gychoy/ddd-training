namespace eShop.Catalog.Services.Errors;

public sealed class ProductNotFoundException(int id)
    : Exception($"Product not found with id {id}")
{
    public int Id { get; } = id;
}
