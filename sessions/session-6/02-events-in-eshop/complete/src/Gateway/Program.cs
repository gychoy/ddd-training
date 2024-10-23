using OpenTelemetry.Log;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("Fusion");

builder.AddServiceDefaults();

builder.Services
    .AddFusionGatewayServer()
    .ConfigureFromCloud(x =>
    {
        x.Stage = "dev";
        x.ApiId = "QXBpCmc2MTZmMGFkMjU2ZDQ0Y2ZkYjE5NDI5YTE2M2JkMjI4Nw==";
        x.ApiKey = "N3wzBUlGLpfZtF9RoJPa38GSG6ympAbzdXrznDNa0LgB3alaqWfQB7iDtv41o4fk";
    })
    .CoreBuilder.AddInstrumentation();

builder.Services.AddLogging(x => x.AddNitroExporter());
builder.Services
    .AddOpenTelemetry()
    .WithTracing(x => x.AddNitroExporter());

var app = builder.Build();

app.MapGraphQL();

app.RunWithGraphQLCommands(args);