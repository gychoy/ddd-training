using eShop.Catalog.Application.Products.Models;
using eShop.Catalog.Entities.Products;
using GreenDonut;

namespace eShop.Catalog.Application.Products.Contracts;

public interface IProductByIdDataLoader
    : IDataLoader<int, ProductDto>;
