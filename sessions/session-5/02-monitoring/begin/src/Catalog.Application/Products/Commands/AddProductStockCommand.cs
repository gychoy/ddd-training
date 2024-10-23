using eShop.Catalog.Application.Products.Models;
using eShop.Catalog.Entities.Products;
using eShop.Catalog.Entities.Products.Errors;
using MediatR;

namespace eShop.Catalog.Application.Products.Queries;

public sealed record AddProductStockCommand(int ProductId, int Quantity) : IRequest<ProductDto>;

public sealed class AddProductStockCommandHandler(IProductRepository repository) :
    IRequestHandler<AddProductStockCommand, ProductDto>
{
    public async Task<ProductDto> Handle(
        AddProductStockCommand request,
        CancellationToken cancellationToken)
    {
        var (productId, quantity) = request;

        var product = await repository.GetProductAsync(productId, cancellationToken);

        if (product == null)
        {
            throw new ProductNotFoundException(productId);
        }

        product.AddStock(quantity);

        repository.UpdateProduct(product);
        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return product.ToReadModel();
    }
}