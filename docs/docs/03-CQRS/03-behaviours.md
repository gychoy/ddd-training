# Behaviors in Mediator

## Introduction

In a CQRS-based system, **Behaviors** in Mediator are a powerful way to add cross-cutting concerns, such as logging, validation, and transaction management, to your commands and queries. 

Instead of adding this logic to each individual handler, **Behaviors** allow you to encapsulate this logic in reusable, centralized components. 
One common use case for behaviors is ensuring that each command runs within a **transaction**, making sure that all changes to the system happen atomically.

## Learning Objectives

By the end of this section, you will be able to:
- Understand what Mediator Behaviors are and why they are useful.
- Implement a **Transaction Behavior** to ensure consistency across your commands.
  
## What Are Mediator Behaviors?

**Behaviors** in MediatR act like middleware for your command and query pipelines. When you send a command or query, behaviors are executed before and after the command/query handler itself. 
This provides a flexible mechanism to add logic that should run on every command or query, such as:

- **Logging**: Capture details about every request and its outcome.
- **Transactions**: Ensure that commands run within a transaction, so that all changes are either committed together or rolled back.
- (**Validation**: Ensure that commands and queries meet certain criteria before being processed.)

Behaviors allow you to **keep your command and query handlers clean** by moving these cross-cutting concerns out of the handlers and into reusable behavior components.

## In a perfect world...

Imagine that every time you process a command, you want to ensure that all related operations either **succeed together** or **fail together**. For example, in the e shop, when you create an order, you might need to:

- Deduct the item quantity from the inventory.
- Record the order details in the database.
- Trigger an email confirmation.

If any part of this process fails, the entire operation should be rolled back, leaving the system in a consistent state.

This is where a **Transaction Behavior** comes in.

By implementing a **Transaction Behavior**, you can wrap your commands in a database transaction without adding transaction management logic to every handler. 
This ensures that if a command handler encounters an error, all changes made by the command are rolled back.

Now, each command is a Unit of Work that either succeeds completely or fails completely. As we have registered the DBContext as a scoped service and all our command handlers are scoped as well, the transaction will be scoped to the command handler.

If we wrap all command handlers in a transaction, we have a consistent way of handling transactions across the application and know that each command will either succeed or fail as a whole. We have **very clear transaction boundaries**.

## Implementing a Transaction Behavior

### Step 1: Define the Transaction Behavior

In this step, you will create a behavior that wraps command execution in a database transaction. This behavior will:
- Begin a transaction before the command handler runs.
- Commit the transaction if the handler completes successfully.
- Roll back the transaction if the handler throws an exception.

Create a new class `TransactionBehavior`:

```csharp
public class TransactionBehavior<TRequest, TResponse>(ICatalogDataContext context)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (context.HasActiveTransaction)
        {
            return await next();
        }

        var strategy = context.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(
            ct => ExecuteWithTransactionAsync(context, next, ct),
            cancellationToken);
    }

    private static async Task<T> ExecuteWithTransactionAsync<T>(
        ICatalogDataContext ctx,
        RequestHandlerDelegate<T> next,
        CancellationToken ct)
    {
        await using var transaction = await ctx.BeginTransactionAsync(ct);
        var response = await next();
        await transaction.CommitAsync(ct);
        return response;
    }
}
```

### Step 2: Register the Behavior in the Dependency Injection Container

Once you have defined the `TransactionBehavior`, you need to make MediatR aware of it by registering it in the `AddApplicationLayer` method:

```csharp
    internal static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddMediatR(
            c =>
            {
                c.RegisterServicesFromAssemblyContaining<ICatalogDataContext>();
                c.Lifetime = ServiceLifetime.Scoped;
                c.AddOpenBehavior(typeof(TransactionBehavior<,>));
            });
        return services;
    }
```

## Why Use a Transaction Behavior?

### 1. Consistency Across Transactions
Using a **Transaction Behavior** ensures that all commands are run within a consistent, transactional scope. This is especially important for complex commands that affect multiple parts of the system:
- If one part of the command fails (e.g., saving to the database), the entire transaction is rolled back, preventing partial updates.
- This guarantees that the system remains in a valid state, avoiding data corruption or inconsistencies.

### 2. Less Complex Application Code
By moving the transaction management logic into a behavior, your command handlers become simpler and more focused on their specific tasks:
- Handlers no longer need to deal with starting, committing, or rolling back transactions.
- This reduces code duplication and makes your application code easier to maintain.

### 3. Clear Boundaries
The **Transaction Behavior** defines a clear boundary for each command:
- You know exactly where the transaction starts and ends, which makes it easier to reason about how commands affect the system.
- This also helps with debugging and troubleshooting, as you can easily track when a transaction begins and what happens if it fails.

## Tasks

1. *Follow the steps outlined above to implement a **Transaction Behavior** in your application.*
2. Implement a logging behavior that captures details about each command/query execution and logs them.
3. Think about validation behaviors. How would you implement a behavior that validates the input of each command/query before it is processed? How would you handle validation errors? Is this validation enough?
