using eShop.IntegrationEvents;
using eShop.IntegrationEvents.EntityFramework.Postgres;

namespace eShop.Ordering.Infrastructure;

internal sealed class IntegrationEventPublisher(DbContext context)
    : IIntegrationEventPublisher
{
    /// <inheritdoc />
    public async Task PublishAsync<T>(
        T integrationEvent,
        CancellationToken cancellationToken = default)
        where T : IntegrationEvent
        => await context.PublishEventAsync(integrationEvent, cancellationToken);

    /// <inheritdoc />
    public async Task PublishAsync<T>(
        ReadOnlyMemory<T> integrationEvents,
        CancellationToken cancellationToken = default)
        where T : IntegrationEvent
        => await context.PublishEventsAsync(integrationEvents, cancellationToken);
}
