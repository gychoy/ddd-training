using eShop.Catalog.Application.ProductTypes.Models;
using GreenDonut;

namespace eShop.Catalog.Application.ProductTypes.DataLoader;

public interface IProductTypeByNameDataLoader 
    : IDataLoader<string, ProductTypeDto>;