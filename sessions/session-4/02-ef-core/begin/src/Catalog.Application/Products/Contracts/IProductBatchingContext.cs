namespace eShop.Catalog.Application.Products.Contracts;

public interface IProductBatchingContext
{
    IProductsDataLoader Products { get; }
    
    IProductByIdDataLoader ProductById { get; }
    
    IProductsByBrandDataLoader ProductsByBrand { get; }
    
    IProductsByTypeDataLoader ProductsByType { get; }
}
