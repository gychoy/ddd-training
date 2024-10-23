using eShop.Catalog.Application.Products.Errors;
using eShop.Catalog.Entities.Products;
using MediatR;

namespace eShop.Catalog.Application.Products.Services;

public sealed record AddProductStockCommand(int ProductId, int Quantity) : IRequest<Product>;

public sealed class AddProductStockCommandHandler(IProductRepository repository) :
    IRequestHandler<AddProductStockCommand, Product>
{
    public async Task<Product> Handle(
        AddProductStockCommand request,
        CancellationToken cancellationToken)
    {
        var (productId, quantity) = request;

        var product = await repository.GetProductAsync(productId, cancellationToken);

        if (product == null)
        {
            throw new ProductNotFoundException(productId);
        }

        if (product.AvailableStock + quantity > product.MaxStockThreshold)
        {
            throw new ProductMaxStockThresholdReachedException(
                productId,
                product.MaxStockThreshold,
                product.AvailableStock,
                quantity);
        }

        product.AvailableStock += quantity;
        product.OnReorder = false;
        repository.UpdateProduct(product);
        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return product;
    }
}