using eShop.Catalog.Entities.Brands;
using eShop.Catalog.Entities.Brands.Errors;
using eShop.Catalog.Events;
using MediatR;

namespace eShop.Catalog.Application.Brands.Handlers;

public sealed class UpdateTotalProductCountOnBrandHandler(IBrandRepository repository) :
    INotificationHandler<ProductCreatedEvent>
{
    public async Task Handle(ProductCreatedEvent notification, CancellationToken cancellationToken)
    {
        var brand = await repository.GetBrandAsync(notification.Product.BrandId, cancellationToken);

        if (brand is null)
        {
            throw new BrandNotFoundException(notification.Product.BrandId);
        }

        brand.IncreaseTotalProducts();

        repository.UpdateBrand(brand);

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}