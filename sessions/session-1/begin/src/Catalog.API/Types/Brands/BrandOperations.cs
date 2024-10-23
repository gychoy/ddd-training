using eShop.Catalog.Models;
using eShop.Catalog.Services;
using eShop.Catalog.Services.Errors;
using HotChocolate.Pagination;
using HotChocolate.Types.Pagination;

namespace eShop.Catalog.Types.Brands;

public static class BrandOperations
{
    [Query]
    [NodeResolver]
    public static async Task<Brand?> GetBrandByIdAsync(
        int id,
        BrandService brandService,
        CancellationToken ct)
        => await brandService.GetBrandByIdAsync(id, ct);
    
    [Query]
    public static async Task<Brand?> GetBrandByNameAsync(
        string name,
        BrandService brandService,
        CancellationToken ct)
        => await brandService.GetBrandByNameAsync(name, ct);

    [Query]
    [UsePaging]
    public static async Task<Connection<Brand>> GetBrandsAsync(
        PagingArguments args,
        BrandService brandService,
        CancellationToken ct)
        => await brandService.GetBrandsAsync(args, ct).ToConnectionAsync();
        

    [Mutation]
    public static async Task<Brand> CreateBrand(
        string name,
        BrandService brandService,
        CancellationToken ct)
    {
        var brand = new Brand { Name = name };
        await brandService.CreateBrandAsync(brand, ct);
        return brand;
    }

    [Mutation]
    [Error<BrandNotFoundException>]
    public static async Task<Brand> RenameBrand(
        int id,
        string name,
        BrandService brandService,
        CancellationToken ct)
    {
        await brandService.RenameBrandAsync(id, name, ct);
        return (await brandService.GetBrandByIdAsync(id, ct))!;
    }
}