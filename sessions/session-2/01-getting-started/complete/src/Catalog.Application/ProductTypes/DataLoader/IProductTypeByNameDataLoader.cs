using eShop.Catalog.Entities.ProductTypes;
using GreenDonut;

namespace eShop.Catalog.Application.ProductTypes.DataLoader;

public interface IProductTypeByNameDataLoader 
    : IDataLoader<string, ProductType>;