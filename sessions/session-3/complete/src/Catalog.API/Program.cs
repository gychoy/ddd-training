var builder = WebApplication.CreateBuilder(args);

builder
    .AddApplicationLayer()
    .AddInfrastructureLayer()
    .AddApiLayer();

builder
    .AddGraphQL()
    .AddCatalogTypes()
    .AddGraphQLConventions()
    .InitializeOnStartup();

var app = builder.Build();

await app.SeedDataAsync();

app.UseCors(c => c.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
app.MapGraphQL();

app.RunWithGraphQLCommands(args);
