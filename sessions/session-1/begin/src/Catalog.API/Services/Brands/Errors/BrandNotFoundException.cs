namespace eShop.Catalog.Services.Errors;

public sealed class BrandNotFoundException(int id)
    : Exception($"Brand not found with id {id}")
{
    public int Id { get; } = id;
}
