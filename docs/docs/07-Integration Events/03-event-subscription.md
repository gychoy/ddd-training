# Integration Event Subscription

## Learning Objectives
By the end of this lesson, you will be able to:
- Understand how to subscribe to an integration event in the eShop application.

## Introduction

Currently, the basket in our eShop application stores the `UnitPrice` of a product at the time it is added to the basket. 
However, product prices may change over time, which means that the stored price in the basket could become outdated. 
To handle this, we will subscribe to the `ProductPriceChangedIntegrationEvent` and update the basket with the new price whenever a product’s price changes.

## Subscribing to Integration Events

The eShop application uses a custom implementation of an **event bus** that allows services to subscribe to integration events. 
Since we are already publishing the `ProductPriceChangedIntegrationEvent` whenever a product's price changes, we can subscribe to this event in the **Basket Service** to keep the basket's prices up to date.

### Step 1: Implement the Event Handler

First, we need to create a handler that listens for the `ProductPriceChangedIntegrationEvent` and updates the corresponding items in the basket:

```csharp
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
```

In this handler:
- We receive the `ProductPriceChangedIntegrationEvent`, which contains the `ProductId` and the new `UnitPrice`.
- We update all basket items where the `ProductId` matches, setting their `UnitPrice` to the new price.

### Step 2: Subscribe to the Event

Next, we need to register the event handler so that the **Basket Service** can subscribe to the `ProductPriceChangedIntegrationEvent`. 
To do this, we add the subscription to the `Program.cs` file:

```csharp
builder
    .AddRabbitMQEventBus()
    .AddSubscription<
        ProductPriceChangedIntegrationEvent,
        ProductPriceChangedIntegrationEventHandler>();
```

This code configures the event bus to subscribe to the `ProductPriceChangedIntegrationEvent` and associates it with the `ProductPriceChangedIntegrationEventHandler`.

### Step 3: Test the Subscription

Now, whenever the price of a product changes, the basket will automatically update the corresponding item's `UnitPrice` in real time. This ensures that the basket always reflects the latest prices.


--- 

## Tasks

1. **Implement the `ProductPriceChangedIntegrationEventHandler`**:
   - Write an event handler that listens for the `ProductPriceChangedIntegrationEvent` and updates the basket items with the new price.
2. What happens when the event handler fails to update the basket items? What would we need to do to handle this scenario?
3. How can we guarantee that the event is processed if multiple services are interested in the same event?