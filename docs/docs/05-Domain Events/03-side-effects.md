# Side Effects in Domain Events

## Side Effects

### Introduction to Side Effects

**Side effects** in software systems refer to actions or changes that occur as a result of an event or function execution but outside the scope of that function. 
In the context of **Domain-Driven Design**, **side effects** are typically triggered by **Domain Events** and can include things like updating related data, notifying external systems, or triggering further actions within the application.

In a CQRS and DDD architecture, side effects are generally decoupled from the original command or query. Instead, they are triggered by **Domain Events**. 
This separation helps maintain a clean and modular architecture where each component has a single responsibility.

### How Side Effects Work in DDD

When a **Domain Event** occurs, it signifies a meaningful change in the system. 
This event often prompts other parts of the system to take action. 
These actions are the **side effects** of the event.

For example, if an `ProductCreatedEvent` is raised, we want to update the total product count for the associated brand. This update is a side effect of the product creation.

In essence, side effects are actions that are triggered in response to **Domain Events**, allowing other aggregates to respond to important changes without coupling those responses directly to the original action. They aggregates do not know about each other.

### Types of Side Effects

Side effects can be categorized based on the nature of the actions they trigger:

1. **Internal Side Effects** (Domain Events):
   - These are changes within the same system or domain. For example, updating related data, adjusting counters, or triggering other domain events.
   - Example: When a `ProductCreated` event is raised, the total number of products for the associated brand might be updated within the same bounded context.

2. **External Side Effects** (Integration Events):
   - These involve interactions with external systems, services, or bounded contexts. This could include sending notifications, updating external systems, or emitting integration events for other microservices.
   - Example: When an `OrderPlaced` event is raised, an email notification is sent to the customer, or a message is sent to an external shipping system.

### Example of a Side Effect: Updating a Product Count

Let's consider we have a list of brands in the UI, that displays the total number of products for each brand.
To query the total product count for each brand would be very expensive, so we store this information in the brand entity itself.
To ensure consistency, we need to update the total product count for a brand whenever a new product is created.

We already have a `ProductCreatedEvent` that is raised when a new product is created. Now, we just need to subscribe to this event and update the total product count for the associated brand.

To listen to an event and trigger a side effect, we use a notification handler. This is part of the **MediatR** library, which we use to dispatch domain events and handle side effects. 

The handlers are part of the application layer.

```csharp
public sealed class UpdateTotalProductCountHandler(IBrandRepository repository) :
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
```

### Ensuring Consistency with Side Effects

One important consideration when triggering side effects is ensuring **consistency**. Since domain events and side effects often run inside the same transaction, they are **transactional by nature**. This means:
- **All or nothing**: If the domain event fails, the side effect will not be triggered. If the side effect fails, the transaction can be rolled back, ensuring that partial changes do not occur.
- **Atomic operations**: Both the changes to the domain and the side effects are executed as part of the same atomic operation, ensuring that they are either all committed or all rolled back together.

For example:
- If a `ProductCreatedEvent` is raised, but the update to the total product count fails, the product creation itself will be rolled back, ensuring the system doesn’t end up in an inconsistent state.

---

## Tasks

1. Follow the steps outlined in the example to implement side effects for your domain events.
2. Implement the same side effect for `ProductTypes`
3. Think about other side effects that might be triggered by domain events in your application. What did you come up with?