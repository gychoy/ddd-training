using eShop.Catalog.Application.Brands.Models;
using GreenDonut;

namespace eShop.Catalog.Application.Brands.Contracts;

public interface IBrandByNameDataLoader : IDataLoader<string, BrandDto>;