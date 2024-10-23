namespace eShop.Catalog.Application.ProductTypes.DataLoader;

public interface IProductTypeBatchingContext
{
    IProductTypesDataLoader ProductTypes { get; }
    
    IProductTypeByIdDataLoader ProductTypeById { get; }
    
    IProductTypeByNameDataLoader ProductTypeByName { get; }
}