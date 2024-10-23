namespace eShop.Catalog.Application.Brands.Contracts;

public interface IBrandBatchingContext
{
    IBrandsDataLoader Brands { get; }
    
    IBrandByIdDataLoader BrandById { get; }
    
    IBrandByNameDataLoader BrandByName { get; }
}