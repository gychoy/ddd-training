using eShop.Catalog.Application.Products.Errors;
using eShop.Catalog.Entities.Products;
using MediatR;

namespace eShop.Catalog.Application.Products.Commands;

public sealed record ChangeProductPriceCommand(int ProductId, decimal NewPrice) : IRequest<Product>;

public sealed class ChangeProductPriceHandler(IProductRepository repository) :
    IRequestHandler<ChangeProductPriceCommand, Product>
{
    public async Task<Product> Handle(ChangeProductPriceCommand request,
        CancellationToken cancellationToken)
    {
        var (productId, newPrice) = request;


        var product = await repository.GetProductAsync(productId, cancellationToken);

        if (product == null)
        {
            throw new ProductNotFoundException(productId);
        }

        product.Price = newPrice;
        repository.UpdateProduct(product);

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return product;
    }
}