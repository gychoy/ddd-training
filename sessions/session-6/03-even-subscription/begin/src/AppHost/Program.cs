var builder = DistributedApplication.CreateBuilder(args);

var basket = builder.AddProject<Projects.Basket_API>("basket");
var catalog = builder.AddProject<Projects.Catalog_API>("catalog");

builder.AddFusionGateway<Projects.Gateway>("gateway")
    .WithSubgraph(basket)
    .WithSubgraph(catalog);

builder.Build().Compose().Run();