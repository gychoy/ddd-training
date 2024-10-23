# Integration Events in eShop

## Learning Objectives
By the end of this lesson, you will be able to:
- Understand the architecture of integration events in eShop.
- Grasp the **Outbox Pattern** and its importance for reliable message delivery.
- Trigger and publish **Integration Events** within the eShop application.

## Preparations

### Environment Setup

Since this session introduces several new components, it's recommended to start from the **begin folder** of this session to ensure your environment is set up correctly.

1. **Open the project folder**:
   ```bash
   code sessions/session-6/02-events-in-eshop/begin
   ```

---

## Event Bus and Integration Events

In eShop, **Integration Events** are triggered from the **Application Layer**. 
These events serve as notifications that a significant event has occurred in one service, which other services might need to respond to. 
Integration events are **published** using an **event bus abstraction**, which ensures that the details of how events are delivered (such as whether you're using RabbitMQ, Azure Service Bus, or another system) are hidden from the application layer.

### Architecture Overview

- **Event Bus**: The event bus is responsible for routing events from the publishing service to the appropriate subscribers. It abstracts the specific implementation, such as RabbitMQ or Kafka, from the rest of the application.
- **Integration Events**: These events represent cross-service communications and are defined globally in the system, often in a dedicated project like `IntegrationEvents`.

For example, in your `HostApplicationBuilder`, you configure the infrastructure, including the event bus setup, like this:

```csharp
public static IHostApplicationBuilder AddInfrastructureLayer(
    this IHostApplicationBuilder builder,
    string connectionName = "CatalogDB")
{
    builder.AddNpgsqlDbContext<CatalogContext>(connectionName);
    builder.AddNpgsqlDataSource(connectionName);
    builder.Services.AddInfrastructureLayer();
    //highlight-next-line
    builder.AddRabbitMQEventBus();  
    return builder;
}
```

### Event Bus in the Application Layer

In the **Application Layer**, we only interact with the **event bus abstraction**. 
The application doesn't need to know the details of how events are being transported (e.g., RabbitMQ). 
This decouples the business logic from the infrastructure details.

```csharp
<ProjectReference Include="..\..\IntegrationEvents\IntegrationEvents.csproj" />
<ProjectReference Include="..\EventBus\EventBus.csproj" />
```

The **Integration Events** are defined in a shared project (`IntegrationEvents`) since multiple services may need to use them.

## The Outbox Pattern

### What is the Outbox Pattern?

The **Outbox Pattern** is a design pattern used to ensure reliable delivery of messages (in this case, integration events) when working with databases and event buses. 
It helps avoid missing message when a service attempts to write to the database and publish an event simultaneously, but if a failure occurs between these two actions, the system may end up in an inconsistent state.

### How the Outbox Pattern Works:
- **Step 1**: When a change occurs in the system (e.g., a product's price is updated), instead of directly publishing the integration event to the event bus, the event is stored in an **outbox table** within the same transaction as the database change.
- **Step 2**: After the transaction is committed successfully, a background process reads the events from the outbox table and publishes them to the event bus.
- **Step 3**: Once an event is successfully published, it is marked as processed and removed from the outbox.

This pattern ensures that if the system crashes after the database change but before the event is published, the event will still be published when the system recovers.

### Implementing the Outbox Pattern in eShop

In eShop, the `CatalogContext` has been extended to support the Outbox Pattern:

```csharp
public class CatalogContext : DbContext
{
    public CatalogContext(
        DbContextOptions<CatalogContext> options,
        IConfiguration configuration)
        : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductType> ProductTypes => Set<ProductType>();
    public DbSet<Brand> Brands => Set<Brand>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new BrandEntityTypeConfiguration());
        builder.ApplyConfiguration(new ProductTypeEntityTypeConfiguration());
        builder.ApplyConfiguration(new ProductEntityTypeConfiguration());
        //highlight-next-line
        builder.UseIntegrationEvents();  
    }
}
```

Additionally, the infrastructure layer is configured to handle integration events:

```csharp
public static class CatalogInfrastructureServiceCollectionExtensions
{
    // left out for brevity...
    private static void AddInfrastructureLayer(this IServiceCollection services)
    {
        // left out for brevity...
        services.AddPostgresIntegrationEvents<CatalogContext>();
    }
}
```

Instead of publishing integration events directly to the event bus, we use the `IIntegrationEventPublisher` to publish events to the outbox table.
If the transaction is successful, the events are then published to the event bus.

---

## Publishing an Integration Event

Integration events are often triggered in response to **domain events**. 
For example, when a `ProductPriceChangedEvent` is raised, an integration event should be published to notify external systems or services about the price change.

### Example: Handling Domain Events and Publishing Integration Events

You can implement an event handler for a domain event, which will in turn publish the corresponding integration event:

```csharp
public sealed class PublishProductPriceChangedEventHandler(
    IIntegrationEventPublisher eventPublisher)
    : INotificationHandler<ProductPriceChangedEvent>
{
    public Task Handle(ProductPriceChangedEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new ProductPriceChangedIntegrationEvent(notification.Product.Id, notification.NewPrice);
        return eventPublisher.PublishAsync(integrationEvent, cancellationToken);
    }
}
```

### Verifying the Event Publication

Once the integration event is published, you can verify its in RabbitMQ by accessing the **RabbitMQ Management UI**:

1. Open your browser and navigate to [http://0.0.0.0:15672/#/exchanges](http://0.0.0.0:15672/#/exchanges).
2. Use the following credentials to log in:
   - **Username**: `admin`
   - **Password**: `root`

Here, you can see the events as they are published to RabbitMQ.

---

## Tasks

1. **Implement the `INotificationHandler` for `ProductPriceChangedEvent`**:
   - Write an event handler that listens for the `ProductPriceChangedEvent` domain event and publishes a `ProductPriceChangedIntegrationEvent`.
   
2. **Check if the Event is Published**:
   - After implementing the handler, trigger the `ProductPriceChangedEvent` in your application.
   - Verify that the event is published to the exchange in RabbitMQ by checking the RabbitMQ Management UI.
