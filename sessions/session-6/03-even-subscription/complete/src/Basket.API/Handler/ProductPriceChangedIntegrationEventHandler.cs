using eShop.Basket.API;
using eShop.IntegrationEvents;
using Microsoft.EntityFrameworkCore;

namespace Basket.API.Handler;

public sealed class ProductPriceChangedIntegrationEventHandler(BasketDbContext context)
    : IIntegrationEventHandler<ProductPriceChangedIntegrationEvent>
{
    public async Task Handle(ProductPriceChangedIntegrationEvent integrationEvent)
    {
        var newPrice = decimal.ToDouble(integrationEvent.NewPrice);

        await context.BasketItems.Where(bi => bi.ProductId == integrationEvent.ProductId)
            .ExecuteUpdateAsync(x => x.SetProperty(y => y.UnitPrice, newPrice));
    }
}