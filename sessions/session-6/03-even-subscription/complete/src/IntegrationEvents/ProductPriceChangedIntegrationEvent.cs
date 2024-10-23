namespace eShop.IntegrationEvents;

public sealed record ProductPriceChangedIntegrationEvent(
    int ProductId,
    decimal NewPrice)
    : IntegrationEvent;