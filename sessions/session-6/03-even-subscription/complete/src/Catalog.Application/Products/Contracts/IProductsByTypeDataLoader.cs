using eShop.Catalog.Application.Products.Models;
using eShop.Catalog.Entities.Products;
using GreenDonut;
using HotChocolate.Pagination;

namespace eShop.Catalog.Application.Products.Contracts;

public interface IProductsByTypeDataLoader 
    : IDataLoader<int, Page<ProductDto>>;