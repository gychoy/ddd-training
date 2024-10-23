# Queries in CQRS

## Introduction

In this session, we’ll explore the **Query** side of CQRS, which focuses on **data retrieval** without modifying the state of the system. 
Queries are for efficiently fetching data, and by separating queries from commands, we can optimize the application for performance and scalability. 
In this section, we’ll discuss how queries are designed, their benefits, and how to implement them using HotChocolate GraphQL.

## Learning Objectives

By the end of this lesson, you will be able to:
- Understand how queries are designed and implemented in a CQRS-based application.
- Recognize the benefits of separating queries from commands in terms of performance and scalability.
- Implement queries in HotChocolate GraphQL.
  
## What Are Queries in CQRS?

In CQRS, **Queries** are responsible for retrieving data from the system. 
They form the **read model** of the application and are designed to fetch data without causing any side effects. 
Unlike commands, which modify the system’s state, queries are **idempotent**—executing the same query multiple times will always yield the same result (assuming the underlying data hasn’t changed).

## Benefits of Using Queries

### 1. Side Effect Free
Queries are designed to have **no side effects**:
- They only fetch data and never modify the underlying state of the system, making them safer and more predictable.
- The side-effect-free nature of queries means they can be run multiple times without any risk of altering the system’s behavior or data integrity.

### 2. Less Complex Application Code
Queries are responsible for **reading** data, and by separating them from the **write** operations (commands), we reduce complexity in the application:
- The logic for reading data is isolated, making the code easier to maintain and understand.
- Queries focus on data retrieval, while commands focus on business logic related to state changes. This separation of concerns ensures that the code for each task is simpler and more focused.
- Developers can easily navigate the code to find where data is being fetched and where business logic for changes resides.

### 3. Optimized for Read Performance
By having a dedicated **read model**, queries can be optimized for performance. From optimized SQL queries to NoSQL databases, the read model can be tailored to meet the specific needs of the application:
- Data for reads can be denormalized or aggregated specifically for queries, reducing the need for complex joins or transformations.
- You can utilize techniques like **caching**, **read replicas**, or **NoSQL databases** for querying data efficiently.
- This separation also allows the read model to evolve independently, giving you the flexibility to fine-tune query performance without affecting the write model.

### 4. Independent Scalability
The read side can be scaled independently from the write side, which is especially useful in read-heavy applications **(most applications are read-heavy)**:
- Since the read model and query side handle only data retrieval, you can scale resources (e.g., databases, caching layers) to meet high demand without needing to scale the entire system.
- This allows for more efficient use of infrastructure, especially in systems where reading operations far outnumber writes.

## Read Models 

Currently, we are exposing entities directly to the client, which is not ideal as it blurs the distinction between the **read** and **write** models. To address this, we need to refactor several parts of the application and introduce **DTOs** (Data Transfer Objects) to manage what is exposed to the client.

Here’s what needs to be done:

- **DTOs**: We need to create DTOs for all entities. These DTOs should only include the data necessary for the client, providing a cleaner and safer data model.
    - `required` fields are the perfect way to ensure that everywhere the DTO is used, it is mapped correctly.
```csharp
public class BrandDto
{
    public required int Id { get; set; }

    [Required]
    public required string Name { get; set; }
}

public static class BrandDtoExtensions
{
    public static BrandDto ToReadModel(this Brand brand)
        => new()
        {
            Id = brand.Id,
            Name = brand.Name
        };
}
```
  
- **DataLoader Changes**: DataLoaders are responsible for fetching data. We need to update them to return DTOs instead of entities.
  
  ```csharp
    internal sealed class BrandsDataLoader(CatalogContext context)
        : IBrandsDataLoader
    {
        public async Task<Page<BrandDto>> LoadAsync(
            PagingArguments pagingArgs,
            CancellationToken cancellationToken = default)
            => await context.Brands
                .AsNoTracking()
                // it's important to project to DTOs before paging
                .Select(x => new BrandDto { Id = x.Id, Name = x.Name })
                .OrderBy(t => t.Name)
                .ThenBy(t => t.Id)
                .ToPageAsync(pagingArgs, cancellationToken);
    }
    ```

- **Command**: Commands should return either a DTO or just the entity’s ID. In this case, we will return the **read models** since the data is already loaded in the process.
```csharp

public sealed class CreateBrandCommandHandler(IBrandRepository repository)
    : IRequestHandler<CreateBrandCommand, BrandDto>
{
    public async Task<BrandDto> Handle(
        CreateBrandCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(request.Name);

        var brand = new Brand { Name = request.Name };

        repository.AddBrand(brand);

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        //highlight-next-line
        return brand.ToReadModel();
    }
}
```

- **GraphQL Changes**:
  - All resolvers that expose entities should be updated to return DTOs.
  - Any `[Parent]` annotations should refer to DTOs, not entities.
  - In `*Node` classes (e.g., `BrandNode`), override the `Configure` method to change the name of the type back to `Brand` (instead of `BrandDto`), as this will reflect in the GraphQL schema.
  - **Important**: The `[ID<Brand>]` field should still reference the **write model**, even though the DTO is used for other data.

```csharp
public static class BrandOperations
{
    [Mutation]
    [Error<BrandNotFoundException>]
    public static async Task<BrandDto> RenameBrand(
        [ID<Brand>] int id,
        string name,
        IMediator mediator,
        CancellationToken ct)
    {
        return await mediator.Send(new RenameBrandCommand(id, name), ct);
    }
}
```

## Designing Queries 

In CQRS, queries are, like the commands, placed in the **Application Layer**. 
Similar to commands, they are designed as **request-response** operations, but they focus purely on fetching data.
Queries are typically **read-only** operations, and they interact with the **read model** to provide optimized data to the client.

### Example Query Design

Now that we have moved all our write operations to commands, we can focus on designing queries for reading data.
The services in the application still contain the logic for fetching data.

By using MediatR for commands, we already have a great mechanism for decoupling queries from implementation and we can reuse the same pattern for queries.
1. **Define the Query interfaces**:
    Instead of extending from `IRequest`, it makes sense to define a new interface `IQuery<TResponse>` that extends from `IRequest<TResponse>`. This way, we can easily distinguish between commands and queries in the behaviours.
     ```csharp
        public interface IQuery;

        public interface IQuery<out TResponse> : IQuery, IRequest<TResponse>;
     ```

2. **Define the Query**: 
     ```csharp
     public sealed record GetProductsQuery(Guid CustomerId) : IQuery<ProductDto>;
     ```

3. **Create the Query Handler**:
    We want to use data loaders in the queries so we can load data efficiently in parallel.
     ```csharp
        public sealed class GetProductsQueryHandler(
            IProductsDataLoader products)
            : IRequestHandler<GetProductsQuery, Page<ProductDto>>
        {
            public async Task<Page<Product>> Handle(
                GetProductsQuery request,
                CancellationToken cancellationToken)
                => await products.LoadAsync(
                    request.PagingArguments,
                    request.Selector,
                    cancellationToken);
        }
     ```

## Queries as GraphQL Queries 

In GraphQL, **queries** represent the read operations. 
They align directly with the CQRS read model, making it straightforward to integrate CQRS queries into your GraphQL schema.

The object types in GraphQL represent the read model of our application, and queries are used to fetch data from these types.

To implement a query in HotChocolate, you can define a query resolver that interacts with the Mediator to dispatch the query:

Open the `ProductOperations` and adjust the `GetProducts` query to dispatch the `GetProductsQuery`:
```csharp
public static class ProductOperations
{
    [Query]
    [UsePaging]
    public static async Task<Connection<ProductDto>> GetProductsAsync(
        PagingArguments args,
        IMediator mediator,
        CancellationToken ct)
        => await mediator.Send(new GetProductsQuery(args), ct)
            .ToConnectionAsync();
}
```

This query can now be called from a GraphQL client, fetching data from the system without causing any side effects.

## Tasks

1. *Follow the steps in the example query design to create a query and query handler in your application.*

2. Apply the same pattern to other queries in your application, ensuring that the read model is optimized for performance and scalability.

3. Are there any changes we have to do to the `TransactionBehavior`? What would be the issue if we don't make any changes?

4. Think about how you could optimize the read model for your application. Consider techniques like caching, denormalization, or read replicas to improve query performance. How would you implement these optimizations in your application?
