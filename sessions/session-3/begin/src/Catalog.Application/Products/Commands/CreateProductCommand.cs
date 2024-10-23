using eShop.Catalog.Application.Products.Errors;
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
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.InitialPrice,
            BrandId = request.BrandId,
            TypeId = request.TypeId,
            RestockThreshold = request.RestockThreshold,
            MaxStockThreshold = request.MaxStockThreshold
        };

        ArgumentException.ThrowIfNullOrEmpty(product.Name);

        if (product.RestockThreshold >= product.MaxStockThreshold)
        {
            throw new MaxStockThresholdToSmallException(product.RestockThreshold,
                product.MaxStockThreshold);
        }

        repository.AddProduct(product);
        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return product.ToReadModel();
    }
}