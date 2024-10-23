using eShop.Catalog.Entities.ProductTypes;
using eShop.Catalog.Entities.ProductTypes.Errors;
using eShop.Catalog.Events;
using MediatR;

namespace eShop.Catalog.Application.ProductTypes.Handlers;

public sealed class UpdateTotalProductCountOnProductTypeHandler(IProductTypeRepository repository) :
    INotificationHandler<ProductCreatedEvent>
{
    public async Task Handle(ProductCreatedEvent notification, CancellationToken cancellationToken)
    {
        var productType =
            await repository.GetProductTypeAsync(notification.Product.TypeId, cancellationToken);

        if (productType is null)
        {
            throw new ProductTypeNotFoundException(notification.Product.TypeId);
        }

        productType.IncreaseTotalProducts();

        repository.UpdateProductType(productType);

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}