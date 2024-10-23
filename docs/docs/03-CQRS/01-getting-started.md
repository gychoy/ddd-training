# Getting Started with CQRS

## Introduction

This section introduces the core concepts of CQRS, outlines its benefits, and walks you through setting up the Mediator pattern in your application. 
By the end of this session, you will be able to execute your first command using Mediator.

## Learning Objectives

By the end of this lesson, you will be able to:
- Understand the basics of CQRS and its components.
- Set up **Mediator** in a C# application.
- Run your first command using Mediator.
  
## Preparations

### Environment Setup

Before we dive into coding, let’s set up the development environment.

1. **Open the Project Folder:**
   Navigate to your project directory and open your code editor:
   ```bash
   code sessions/session-2/getting-started/begin
   ```

2. **Install Dependencies:**
   Ensure all necessary packages are installed. Open a terminal in your project directory and run:
   ```bash
   dotnet restore
   ```

3. **Run the Project:**
   After the dependencies are installed, start your project by running:
   ```bash
   dotnet run
   ```

4. **Verify the Setup:**
   You should see the application running successfully without errors. This means everything is ready for you to begin implementing CQRS with Mediator.

## What is CQRS and Why Should I Use It?

CQRS (Command Query Responsibility Segregation) is a pattern that separates write operations (commands) from read operations (queries). 
This approach allows for more flexibility and optimization, especially in complex systems. 

CQRS helps you:

- **Optimize performance**: You can independently scale the read and write sides.
- **Maintain clear separation of concerns**: Commands and Queries have distinct roles.
- **Increase flexibility**: Each side can be designed independently to meet different needs (e.g., command side optimized for consistency, query side for performance).

## Setting up Mediator

### What is Mediator?

The **Mediator** pattern is used to decouple the sender and receiver of a request. 
In CQRS, Mediator is typically responsible for dispatching commands or queries to their respective handlers. 
This pattern helps simplify interactions between different components of the system.

### Installing Mediator

First, you need to install the Mediator package to integrate it into your application.

MediatR is a popular library that provides a simple implementation of the Mediator pattern in C#.

1. Install **MediatR** in `Catalog.Application`:
   ```bash
   dotnet add package MediatR
   ```

2. Register the Mediator Services in `AddApplicationLyaer`
   ```csharp
    public static class OrderingApplicationServiceCollectionExtensions
    {
        public static IHostApplicationBuilder AddApplicationLayer(
            this IHostApplicationBuilder builder)
        {
            builder.Services.AddApplicationLayer();
            return builder;
        }

        internal static IServiceCollection AddApplicationLayer(this IServiceCollection services)
        {
            services.AddScoped<BrandService>();
            services.AddScoped<ProductService>();
            services.AddScoped<ProductTypeService>();
            services.AddMediatR(
                c =>
                {
                    c.RegisterServicesFromAssemblyContaining<ICatalogDataContext>();
                    c.Lifetime = ServiceLifetime.Scoped;
                });
            return services;
        }
    }
   ```

### Creating Your First Command

Commands are requests to perform an action that changes the state of the system. 
Let's create a simple command for our project.

1. **Define the Command**: Create a new file call `HelloWorldCommand.cs`:
   ```csharp
   public sealed record HelloWorldCommand : IRequest<string>;
   ```

2. **Create the Command Handler**: Every command needs a handler that processes the request. You can either put the handler in the same file or create a separate file for it.
   ```csharp
   public sealed class HelloWorldCommandHandler : IRequestHandler<HelloWorldCommand, string>
   {
       public Task<string> Handle(HelloWorldCommand request, CancellationToken cancellationToken)
       {
           return Task.FromResult("Hello, World!");
       }
   }
   ```

3. **Dispatching the Command**: Now, let's dispatch the command using the Mediator. You can do this in a query and dispatch the command from there.
   ```csharp
   public static class BrandOperations
   {
        [Query]
        public static async Task<string?> HelloWorld(
            IMediator mediator,
            CancellationToken ct)
            => await mediator.Send(new HelloWorldCommand(), ct);
   }
   ```

4. **Run the Command**: Open Nitro and run the `HelloWorld` query. You should see the response `Hello, World!`.
    ```graphql
    query HelloWorld {
        helloWorld
    }
    ```


## Tasks

1. **Set up Mediator in Your Application**: Follow the steps outlined above to install MediatR, create a command, and set up a handler.
2. **Run Your First Command**: Implement the `HelloWorld` and successfully execute it using the Mediator.
3. **Add an input**: Modify the code so that you can send `{ helloWorld(name: "Alice") }` and get `Hello, Alice!` as a response.