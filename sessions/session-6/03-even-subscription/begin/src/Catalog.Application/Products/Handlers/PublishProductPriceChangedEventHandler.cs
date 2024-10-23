using eShop.Catalog.Events;
using eShop.IntegrationEvents;
using MediatR;

namespace eShop.Catalog.Application.Products.Handlers;

public sealed class PublishProductPriceChangedEventHandler(
    IIntegrationEventPublisher eventPublisher)
    : INotificationHandler<ProductPriceChangedEvent>
{
    public Task Handle(ProductPriceChangedEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent =
            new ProductPriceChangedIntegrationEvent(notification.Product.Id, notification.NewPrice);

        return eventPublisher.PublishAsync(integrationEvent, cancellationToken);
    }
}