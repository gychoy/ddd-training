# Clients

## Introduction

With **Nitro**, you can also manage your **GraphQL clients** efficiently. 
Most GraphQL clients can export the queries they use as **persisted documents**, and these documents can then be uploaded to Nitro for storage. 
This enables a more secure and controlled execution of queries on your server, as only the pre-approved queries stored in Nitro can be executed.

In this setup, the client sends the **document ID** to the server, and the server retrieves the corresponding document from Nitro. 
This approach protects your server from **malicious queries** by ensuring that only your authorized queries are executed.

Additionally, when you publish a **Fusion configuration**, Nitro validates that no accidental breaking changes affect your clients.

## Publishing Queries to Nitro

### Step 1: Create a Client

First, create a new client in Nitro. This will allow you to associate persisted documents with that client.

```bash
nitro client create --name "MyClient"
```

### Step 2: Upload Persisted Documents

Next, upload the persisted documents (your GraphQL queries) to Nitro. This step is typically part of your build pipeline, where you export the client queries to Nitro.

```bash
nitro client upload --operations-file ./queries.json --tag 0.0.1 --client-id Q2xpZW50Cmc3M2FhZjE3NjE2Yjg0NGRhYjUwMmFiZTU0NDU3MzhhZA==
```

Here’s an example of the `queries.json` file that contains your persisted queries:

```json title=queries.json
{
    "583150f447dcb671210c76f5eb98a87f": "query MyBasket { me { basket { id items { quantity unitPrice product { name price } } } } }"
}
```

### Step 3: Publish the Client

Before deploying your client application (e.g., to an app store or a web environment), you need to publish the client to a stage in Nitro. 
This step ensures that the client and its queries are recognized by the gateway.

```bash
nitro client publish --client-id Q2xpZW50Cmc3M2FhZjE3NjE2Yjg0NGRhYjUwMmFiZTU0NDU3MzhhZA== --tag 0.0.1 --stage dev
```

Once published:
- The **gateway** knows about all the queries associated with this client.
- The client can now send only the **query ID** to the server, and the server will fetch the associated query from Nitro.
- You can configure your server to only accept persisted queries, locking down the system and rejecting any non-persisted operations.

### Validating Clients During Fusion Configuration

Whenever you publish a **Fusion configuration**, Nitro will also validate that none of the changes break any of the persisted queries for your clients. 
This validation ensures backward compatibility and prevents clients from being affected by schema changes.

--- 

## Tasks

1. **Publish Your Client to Nitro**:
   - Follow the steps outlined above to create a client, upload persisted documents, and publish the client to Nitro.

2. **Validate a Fusion Configuration with a Breaking Change**:
   - For example, introduce a breaking change by removing a field (e.g., `Product.name`) from your schema. See how Nitro handles the validation step and alerts you to the breaking change.
