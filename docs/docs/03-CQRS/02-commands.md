# Commands in CQRS

## Introduction

In this session, we will dive deeper into **Commands** within the CQRS pattern. 

We'll explore how commands are designed, how they fit into the application layer, and their role in abstracting use cases.

Commands are essential for performing operations that change the state of your application, and we'll see how they help achieve separation of concerns, maintain consistency, and simplify the overall architecture.


## Learning Objectives

By the end of this lesson, you will be able to:
- Understand how commands are designed and implemented in a CQRS-based application.
- Know where to place commands in the application layer and how they contribute to a cleaner architecture.
- Implement commands in HotChocolate GraphQL as mutations.
  
## What Are Commands in CQRS?

In CQRS, **Commands** represent actions that change the state of the system. 
These actions could be anything from creating a new order to updating user information. 
Commands are part of the **write model** of the system and focus exclusively on modifying data, unlike queries that only retrieve data.

Commands are handled by **Command Handlers**, which contain the logic to process the action and ensure the necessary changes are made to the system.

## Benefits of Using Commands

### 1. Separation of Concerns
Commands encapsulate business logic related to **state changes**, while queries handle **data retrieval**. This separation:
- Keeps code for reads and writes isolated from each other, reducing the risk of unexpected side effects.
- Allows you to optimize the read and write sides independently, which improves both performance and maintainability.

### 2. Less Complexity in Application Code
By moving business logic into **Command Handlers**, the overall complexity in the application’s core codebase is reduced:
- Use cases are encapsulated in commands, making it easier to understand the flow of operations.
- This makes the code more readable, as it is split into logical units.
- Encourages **single-responsibility principle** by having each command perform one specific action.

### 3. Easier to Navigate
With commands placed in a clear structure, it becomes easier for developers to:
- Find the logic that handles specific actions, as each command has its own dedicated handler. You look for the `createProduct` mutation logic? Look for the `CreateProductCommandHandler`.
- Make changes or extend functionality without disrupting other parts of the code. For example, if you need to update how an order is created, you only need to modify the `CreateProductCommandHandler`.

## Designing Commands 

In CQRS, commands are placed in the **Application Layer**. 
This layer coordinates the flow of information between the Presentation and the Domain Layer. 
Commands are not part of the domain logic itself but serve as the mechanism to trigger domain actions (which you will see in the next session).

### Example Command Design

At the moment all our application logic is inside services. Each service method is a use case.
Over time, as the application grows, it becomes harder to manage and maintain the services and over 5 constructor arguments are not uncommon. The bigger the service dependencies, the more expensive a creation of a service instance becomes. This becomes more managable by using dataloader groups, yet, the service still mixes too many concerns.

```csharp 
public sealed class BrandService(
    IBrandRepository repository,
    IBrandBatchingContext batching)
{
    public async Task<Brand?> GetBrandByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
        => await batching.BrandById.LoadAsync(id, cancellationToken);

    ///...
    public async Task CreateBrandAsync(Brand brand, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(brand.Name))
        {
            ArgumentException.ThrowIfNullOrEmpty(brand.Name);
        }
        
        repository.AddBrand(brand);
        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
```

Commands help to encapsulate these use cases into separate classes, making the code more readable and maintainable. Additionally, the use case can be executed cheaper as the command handler is created only once and reused for each command execution. By having less dependencies in the command handler, it is easier also easier to test.

1. **Define the Command**: 
A command can have as many properties as needed to capture the data required to perform the operation.

We can copy all the arguments from the `CreateProductAsync` method in the `ProductService` and create a command from it.

```csharp
public record CreateProductCommand(
    string Name,
    string? Description,
    decimal InitialPrice,
    int TypeId,
    int BrandId,
    int RestockThreshold,
    int MaxStockThreshold)
    : IRequest<Product>;
```

2. **Create the Command Handler**:
Now that we have defined the command, we need to create a handler that processes the command and performs the necessary operations.

This is essentially the same logic that was in the `CreateProductAsync` method in the `ProductService`.
   
```csharp
public sealed class CreateProductCommandHandler(
    IProductRepository repository)
    : IRequestHandler<CreateProductCommand, Product>
{
    public async Task<Product> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(product.Name);
        
        if (product.RestockThreshold >= product.MaxStockThreshold)
        {
            throw new MaxStockThresholdToSmallException(product.RestockThreshold, product.MaxStockThreshold);
        }
        
        repository.AddProduct(product);
        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
```

## Commands as GraphQL Mutations

In GraphQL, **mutations** represent write operations. 
Since commands in CQRS change state, they naturally align with GraphQL mutations.

The input types in GraphQL are the write model of our application.

In a mutation, we now want to dispatch the command to the Mediator to handle the operation.

Open the `ProductOperations` and adjust the `CreateProduct` mutation to dispatch the `CreateProductCommand`:

   ```csharp
public static class ProductOperations
{
    [Mutation]
    public static async Task<Product> CreateProductAsync(
        string name,
        string? description,
        decimal initialPrice,
        [ID<Product>] int typeId,
        [ID<Brand>] int brandId,
        int restockThreshold,
        int maxStockThreshold,
        IMediator mediator,
        CancellationToken ct)
        => await mediator.Send(new CreateProductCommand(
            name,
            description,
            initialPrice,
            typeId,
            brandId,
            restockThreshold,
            maxStockThreshold), ct);
    }
```

Now the `CreateProduct` mutation is a thin layer that dispatches the command to the Mediator, which in turn invokes the command handler to perform the operation.

In this specific case, the command has so many properties that it is a bit cumbersome to pass them all as separate arguments.

We can also pass the command directly to the `CreateProductAsync` mutation, but have to define an input object in the schema:

```csharp
public static class ProductOperations
{
    [Mutation]
    public static async Task<Product> CreateProductAsync(
        CreateProductCommand input,
        IMediator mediator,
        CancellationToken ct)
        => await mediator.Send(input, ct);
}

public sealed class CreateProductInputType
    : InputObjectType<CreateProductCommand>
{
    protected override void Configure(
        IInputObjectTypeDescriptor<CreateProductCommand> descriptor)
    {
        descriptor.Name("CreateProductInput");
        descriptor.Field(t => t.BrandId).ID<Brand>();
        descriptor.Field(t => t.TypeId).ID<ProductType>();
    }
}
```


## Tasks

1. *Follow the steps outlined above to create a command and its handler for the `CreateProduct` operation.*

2. Apply the same pattern to other operations in your application that modify the state. Try out both approaches: passing individual arguments and using an input object.
