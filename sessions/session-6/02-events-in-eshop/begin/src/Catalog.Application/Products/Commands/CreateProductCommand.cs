using eShop.Catalog.Application.Products.Models;
using eShop.Catalog.Entities.Products;
using MediatR;

namespace eShop.Catalog.Application.Products.Commands;

public sealed record CreateProductCommand(
    string Name,
    string? Description,
    decimal InitialPrice,
    int BrandId,
    int TypeId,
    int RestockThreshold,
    int MaxStockThreshold) : IRequest<ProductDto>;

public sealed class CreateProductCommandHandler(IProductRepository repository)
    : IRequestHandler<CreateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = Product.Create(
            request.Name,
            request.Description,
            request.InitialPrice,
            request.BrandId,
            request.TypeId,
            request.RestockThreshold,
            request.MaxStockThreshold);

        repository.AddProduct(product);
        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return product.ToReadModel();
    }
}