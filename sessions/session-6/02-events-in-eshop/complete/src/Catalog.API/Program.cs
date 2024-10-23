using OpenTelemetry.Log;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder
    .AddApplicationLayer()
    .AddInfrastructureLayer()
    .AddApiLayer();

builder
    .AddGraphQL()
    .AddCatalogTypes()
    .AddGraphQLConventions()
    .AddInstrumentation()
    .AddNitro(x =>
    {
        x.Stage = "dev";
        x.ApiId = "QXBpCmcwYjMzMmI3ZGE0ZWE0ODQxOWE3N2I2MjViYmZmZDZkNA==";
        x.ApiKey = "yC6GMJPiZPVBLKovMYp0ZwHINRXthEzGdWH18n6an1O03S34OzhUgGZH2n3FOiNC";
    })
    .InitializeOnStartup();

builder.Services.AddLogging(x => x.AddNitroExporter());
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing .AddNitroExporter());

var app = builder.Build();


await app.SeedDataAsync();

app.UseCors(c => c.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
app.MapGraphQL();

app.RunWithGraphQLCommands(args);