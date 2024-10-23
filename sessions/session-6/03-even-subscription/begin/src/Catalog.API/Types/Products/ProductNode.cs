using eShop.Catalog.Application.Brands;
using eShop.Catalog.Application.Brands.Queries;
using eShop.Catalog.Application.Products.Models;
using eShop.Catalog.Application.ProductTypes.Models;
using eShop.Catalog.Application.ProductTypes.Queries;
using eShop.Catalog.Entities.Brands;
using eShop.Catalog.Entities.Products;
using eShop.Catalog.Entities.ProductTypes;
using MediatR;

namespace eShop.Catalog.Types.Products;

[ObjectType<ProductDto>]
public static partial class ProductNode
{
    static partial void Configure(IObjectTypeDescriptor<ProductDto> descriptor)
    {
        descriptor.Name("Product");
    }

    [BindMember(nameof(product.BrandId))]
    public static async Task<BrandDto?> GetBrandAsync(
        [Parent] ProductDto product,
        IMediator mediator,
        CancellationToken ct)
        => await mediator.Send(new GetBrandByIdQuery(product.BrandId), ct);

    [BindMember(nameof(product.TypeId))]
    public static async Task<ProductTypeDto?> GetTypeAsync(
        [Parent] ProductDto product,
        IMediator mediator,
        CancellationToken ct)
        => await mediator.Send(new GetProductTypeByIdQuery(product.TypeId), ct);
}