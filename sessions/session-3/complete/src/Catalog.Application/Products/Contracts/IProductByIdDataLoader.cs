using eShop.Catalog.Application.Products.Models;
using GreenDonut;

namespace eShop.Catalog.Application.Products.Contracts;

public interface IProductByIdDataLoader
    : IDataLoader<int, ProductDto>;
