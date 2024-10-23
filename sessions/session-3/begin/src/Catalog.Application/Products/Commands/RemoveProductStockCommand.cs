using eShop.Catalog.Application.Products.Errors;
using eShop.Catalog.Application.Products.Models;
using eShop.Catalog.Entities.Products;
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

        if (quantityDesired <= 0)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(quantityDesired);
        }

        if (product.AvailableStock == 0)
        {
            throw new ProductOutOfStockException(product.Id);
        }

        if (product.AvailableStock < quantityDesired)
        {
            throw new ProductNotEnoughStockException(product.Id, product.AvailableStock, quantityDesired);
        }

        product.AvailableStock -= quantityDesired;

        repository.UpdateProduct(product);
        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return product.ToReadModel();
    }
}