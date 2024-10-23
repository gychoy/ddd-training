using eShop.Catalog.Entities.Brands;
using GreenDonut;

namespace eShop.Catalog.Application.Brands.Contracts;

public interface IBrandByNameDataLoader : IDataLoader<string, BrandDto>;