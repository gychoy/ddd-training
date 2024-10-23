using eShop.Catalog.Application.Products.Models;
using eShop.Catalog.Entities.Products;
using eShop.Catalog.Entities.Products.Exceptions;
using MediatR;

namespace eShop.Catalog.Application.Products.Commands;

public sealed record RemoveProductStockCommand(int Id, int QuantityDesired) 
    : IRequest<ProductDto>;

public sealed class RemoveProductStockCommandHandler(IProductRepository repository) 
    : IRequestHandler<RemoveProductStockCommand, ProductDto>
{
    public async Task<ProductDto> Handle(
        RemoveProductStockCommand request,
        CancellationToken cancellationToken)
    {
        var (productId, quantityDesired) = request;
        var product = await repository.GetProductAsync(productId, cancellationToken);

        if (product == null)
        {
            throw new ProductNotFoundException(productId);
        }

        product.RemoveStock(quantityDesired);

        repository.UpdateProduct(product);
        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return product.ToReadModel();
    }
}