using eShop.Catalog.Application.Products.Models;
using eShop.Catalog.Application.Products.Queries;
using eShop.Catalog.Application.ProductTypes.Models;
using HotChocolate.Pagination;
using HotChocolate.Types.Pagination;
using MediatR;

namespace eShop.Catalog.Types.ProductTypes;

[ObjectType<ProductTypeDto>]
public static partial class ProductTypeNode
{
    static partial void Configure(IObjectTypeDescriptor<ProductTypeDto> descriptor)
    {
        descriptor.Name("ProductType");
    }

    [UsePaging(ConnectionName = "ProductTypeProducts")]
    public static async Task<Connection<ProductDto>> GetProductsAsync(
        [Parent] ProductTypeDto type,
        PagingArguments pagingArgs,
        IMediator mediator,
        CancellationToken ct)
        => await mediator
            .Send(new GetProductsByTypeQuery(type.Id, pagingArgs), ct)
            .ToConnectionAsync();
}