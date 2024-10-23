using eShop.Catalog.Application.Brands.Models;
using eShop.Catalog.Application.Products.Models;
using eShop.Catalog.Application.Products.Queries;
using eShop.Catalog.Entities.Brands;
using HotChocolate.Pagination;
using HotChocolate.Types.Pagination;
using MediatR;

namespace eShop.Catalog.Types.Brands;

[ObjectType<BrandDto>]
public static partial class BrandNode
{
    static partial void Configure(IObjectTypeDescriptor<BrandDto> descriptor)
    {
        descriptor.Name("Brand");
    }

    [UsePaging(ConnectionName = "BrandProducts")]
    public static async Task<Connection<ProductDto>> GetProductsAsync(
        [Parent] BrandDto brand,
        PagingArguments pagingArgs,
        IMediator mediator,
        CancellationToken ct)
        => await mediator
            .Send(new GetProductsByBrandQuery(brand.Id, pagingArgs), ct)
            .ToConnectionAsync();
}