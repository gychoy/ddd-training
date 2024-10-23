# Monitoring

## Introduction

In distributed systems, **monitoring** is both crucial and challenging. Understanding the real-time behavior of your system is essential for diagnosing issues and ensuring smooth operations. Fortunately, **Nitro** provides a built-in monitoring solution that allows you to track your system in real-time. We are almost ready to leverage this capability—there’s just one final step.

## Registering the Exporter

To enable monitoring, you need to register the exporter in your `Program.cs` file. This ensures that the necessary traces and metrics are sent to Nitro for visualization on the dashboard.

### Step 1: Update the `Program.cs` File

Add the following code to your `Program.cs` to register the Nitro exporter:

```csharp
builder.Services
    .AddFusionGatewayServer()
    .ConfigureFromCloud(x =>
    {
        x.Stage = "dev";
        x.ApiId = "QXBpCmc2MTZmMGFkMjU2ZDQ0Y2ZkYjE5NDI5YTE2M2JkMjI4Nw==";
        x.ApiKey = "N3wzBUlGLpfZtF9RoJPa38GSG6ympAbzdXrznDNa0LgB3alaqWfQB7iDtv41o4fk";
    })
    // Don't forget to add instrumentation to the gateway!
    //highlight-next-line
    .CoreBuilder.AddInstrumentation();

builder.Services.AddLogging(x => x.AddNitroExporter());
builder.Services
    .AddOpenTelemetry()
    .WithTracing(x => x.AddNitroExporter());
```

### Step 2: Run the Application

Once the exporter is registered, you can run your application and execute a few queries. These actions will automatically generate traces that are sent to Nitro, where they will be visualized in the **Nitro Dashboard**.

## Monitoring Subgraphs

To view traces from your subgraphs, you need to create APIs for each subgraph and push their traces to Nitro as well. This ensures comprehensive monitoring across all parts of your distributed system.

--- 

## Tasks

1. Push your gateway traces to Nitro by following the steps outlined above.
2. Add a new API for each subgraph in your system to monitor their traces as well.

