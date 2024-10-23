namespace eShop.Catalog.Types.Products;

public sealed record CreateProductInput(
    string Name,
    string? Description,
    decimal InitialPrice,
    int TypeId,
    int BrandId,
    int RestockThreshold,
    int MaxStockThreshold);

public sealed record RenameProductCommand(
    int Id,
    decimal NewPrice);

public sealed record ChangeProductPriceCommand(
    int Id,
    decimal NewPrice);

public sealed record ChangeProductDescriptionCommand(
    int Id,
    decimal NewPrice);

public sealed record RemoveProductStockCommand(
    int Id,
    int QuantityDesired);

public sealed record AddProductStockCommand(
    int Id,
    int Quantity);


    
    