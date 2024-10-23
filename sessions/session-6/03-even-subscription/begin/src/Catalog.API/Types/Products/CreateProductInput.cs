using eShop.Catalog.Application.Products.Commands;
using eShop.Catalog.Entities.Brands;
using eShop.Catalog.Entities.ProductTypes;

namespace eShop.Catalog.Types.Products;

public sealed class CreateProductInputType : InputObjectType<CreateProductCommand>
{
    protected override void Configure(
        IInputObjectTypeDescriptor<CreateProductCommand> descriptor)
    {
        descriptor.Name("CreateProductInput");
        descriptor.Field(t => t.BrandId).ID<Brand>();
        descriptor.Field(t => t.TypeId).ID<ProductType>();
    }
}