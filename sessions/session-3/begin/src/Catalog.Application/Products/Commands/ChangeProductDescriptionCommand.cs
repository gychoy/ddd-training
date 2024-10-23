using eShop.Catalog.Application.Products.Errors;
using eShop.Catalog.Application.Products.Models;
using eShop.Catalog.Entities.Products;
using MediatR;

namespace eShop.Catalog.Application.Products.Commands;

public sealed record ChangeProductDescriptionCommand(int Id, string NewDescription)
    : IRequest<ProductDto>;

public sealed class ChangeProductDescriptionHandler(IProductRepository repository)
    : IRequestHandler<ChangeProductDescriptionCommand, ProductDto>
{
    public async Task<ProductDto> Handle(
        ChangeProductDescriptionCommand request,
        CancellationToken cancellationToken)
    {
        var (productId, newDescription) = request;
        var product = await repository.GetProductAsync(productId, cancellationToken);

        if (product == null)
        {
            throw new ProductNotFoundException(productId);
        }

        product.Description = newDescription;

        repository.UpdateProduct(product);
        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return product.ToReadModel();
    }
}