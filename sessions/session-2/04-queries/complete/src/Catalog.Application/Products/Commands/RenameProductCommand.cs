using eShop.Catalog.Application.Products.Errors;
using eShop.Catalog.Application.Products.Models;
using eShop.Catalog.Entities.Products;
using MediatR;

namespace eShop.Catalog.Application.Products.Commands;

public sealed record RenameProductCommand(int ProductId, string NewName) : IRequest<ProductDto>;

public sealed class RenameProductCommandHandler(IProductRepository repository) :
    IRequestHandler<RenameProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(
        RenameProductCommand request,
        CancellationToken cancellationToken)
    {
        var (productId, newName) = request;

        var product = await repository.GetProductAsync(productId, cancellationToken);

        if (product == null)
        {
            throw new ProductNotFoundException(productId);
        }

        product.Name = newName;
        repository.UpdateProduct(product);

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return product.ToReadModel();
    }
}