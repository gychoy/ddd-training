# Domain Events in EF Core

#  Domain Events in EF Core

## Introduction to Domain Events in EF Core

In DDD, **Domain Events** capture and model significant business changes. When working with **Entity Framework Core**, these events are tightly integrated with the **Unit of Work** and **transactional consistency**. EF Core allows us to dispatch domain events within the same transaction as our data changes, ensuring that all actions remain consistent.

## Learning Objectives
- Integrate Domain Events with EF Core to ensure transactional consistency.
- Understand how to raise and dispatch domain events in EF Core.
- Know where to place domain event handling logic in your application.

## Preparations

### Environment Setup

As there are several new components, we recoomend to use the begin folder of this session to get started.

Navigate to your project directory and open your code editor:
```bash
code sessions/session-4/02-ef-core/begin
```

## EF Core and Domain Events

One of the most popular patterns for dispatching domain events in EF Core is by using **Mediator Notifications**. MediatR simplifies the decoupling of event publishing and handling. Here's how it works in the context of EF Core:
- When changes are made to the aggregates in your domain, those aggregates **raise domain events**.
- Before saving those changes to the database, you **dispatch the domain events** using MediatR.
- Once dispatched, any event handlers registered for those domain events can react accordingly.

### Event Base Class
In the start project of this session, you will find a base class `Event` that all domain events should inherit from.
This class implements the `INotification` interface from MediatR, so it can be dispatched as a notification.

```csharp
/// <summary>
/// Base class for domain events.
/// </summary>
public abstract record Event : INotification;
```

### Entity Base Class
There is also a base class `Entity` that all entities should inherit from. This class contains a collection of domain events that are raised by the entity.

```csharp
/// <summary>
/// Base class for entities
/// </summary>
public abstract class Entity
{
    public EventCollection Events { get; } = [];
}
``` 

### Configuration
As there is now a property on each entity, you need to configure EF Core to ignore this property when mapping the entities to the database. This is done in the configurations like `ProductTypeEntityTypeConfiguration`.

```csharp
internal sealed class ProductTypeEntityTypeConfiguration : IEntityTypeConfiguration<ProductType>
{
    public void Configure(EntityTypeBuilder<ProductType> builder)
    {
        builder
            .ToTable("ProductTypes")
            .HasKey(t => t.Id);

        builder
            .Property(t => t.Id)
            .ValueGeneratedOnAdd();

        builder
            .Property(cb => cb.Name)
            .HasMaxLength(100);

        builder
            .HasIndex(t => t.Name, "IX_ProductTypes_Name")
            .IsUnique();
        
        //highlight-next-line
        builder.Ignore(t => t.Events);
    }
}
```

### Dispatching Domain Events 

Domain events are part of the `SaveChanges()`, and they are dispatched **before** the transaction is committed, making sure that, all domain event handlers run within the same transaction and succeed or fail together.

Therefore, we want to publish the events before the changes are saved to the database:


```csharp title=CatalogDbContext.cs

public sealed class CatalogDataContext(
    CatalogContext context,
    //highlight-next-line
    IMediator mediator)
    : ICatalogDataContext
{
    // left out for brevity...

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        //highlight-next-line
        await mediator.DispatchDomainEventsAsync(context, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
    
    // left out for brevity...
}
```

## Defining Domain Events

Domain Events live in the Domain Layer and are designed as simple `record`'s. 
The event are used to communicate across aggregates and can hold a reference to the entity that triggered the event.
These events are in memory and never serialized!

```csharp
public sealed record BrandCreatedEvent(Brand Brand) : Event;

public sealed record BrandRenamedEvent(Brand Brand, string NewName) : Event;
```

The domain events should be triggered on domain changes and therefor the domain methods like `Brand.Rename` or `ProductType.Create` should raise the domain events.

As all `Entities` have a collection of events, the events should be added to this collection when they are raised.

```csharp
public sealed class Brand : Entity
{
    private Brand()
    {
    }

    public int Id { get; private set; }

    [Required]
    public string Name { get; private set; } = default!;

    public void Rename(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
        Events.Add(new BrandRenamedEvent(this, name));
    }

    public static Brand Create(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var brand = new Brand { Name = name };
        brand.Events.Add(new BrandCreatedEvent(brand));
        return brand;
    }
}

```

## Tasks

1. **Define Your Domain Events**: Create domain events that reflect meaningful changes in your business domain, such as `ProductPriceChanged` or `ProductCreated`.
1. **Raise Domain Events**: Implement the logic to raise domain events in your domain entities when significant changes occur.
1. **Dispatch Domain Events**: Ensure that domain events are dispatched before saving changes to the database, so that all event handlers run within the same transaction.
