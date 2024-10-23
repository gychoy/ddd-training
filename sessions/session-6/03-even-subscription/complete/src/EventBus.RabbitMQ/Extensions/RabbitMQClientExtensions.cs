using System.Diagnostics;
using System.Net.Sockets;
using HealthChecks.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace eShop.EventBus;

/// <summary>
/// Extension methods for connecting to a RabbitMQ message broker.
/// </summary>
internal static class RabbitMQClientExtensions
{
    private const string ActivitySourceName = "RabbitMQ.Client";

    private static readonly ActivitySource _activitySource = new(ActivitySourceName);

    private const string DefaultConfigSectionName = "RabbitMQ:Client";

    /// <summary>
    /// Registers <see cref="IConnection"/> as a singleton in the services provided by the <paramref name="builder"/>.
    /// Enables retries, corresponding health check, logging, and telemetry.
    /// </summary>
    /// <param name="builder">The <see cref="IHostApplicationBuilder" /> to read config from and add services to.</param>
    /// <param name="connectionName">A name used to retrieve the connection string from the ConnectionStrings configuration section.</param>
    /// <param name="configureSettings">An optional method that can be used for customizing the <see cref="RabbitMQClientSettings"/>. It's invoked after the settings are read from the configuration.</param>
    /// <param name="configureConnectionFactory">An optional method that can be used for customizing the <see cref="ConnectionFactory"/>. It's invoked after the options are read from the configuration.</param>
    /// <remarks>Reads the configuration from "RabbitMQ:Client" section.</remarks>
    public static void AddRabbitMQClient(
        this IHostApplicationBuilder builder,
        string connectionName,
        Action<RabbitMQClientSettings>? configureSettings = null,
        Action<ConnectionFactory>? configureConnectionFactory = null)
        => AddRabbitMQClient(builder, configureSettings, configureConnectionFactory,
            connectionName);

    private static void AddRabbitMQClient(
        IHostApplicationBuilder builder,
        Action<RabbitMQClientSettings>? configureSettings,
        Action<ConnectionFactory>? configureConnectionFactory,
        string connectionName)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var configSection = builder.Configuration.GetSection(DefaultConfigSectionName);
        var namedConfigSection = configSection.GetSection(connectionName);

        var settings = new RabbitMQClientSettings();
        configSection.Bind(settings);
        namedConfigSection.Bind(settings);

        if (builder.Configuration.GetConnectionString(connectionName) is string connectionString)
        {
            settings.ConnectionString = connectionString;
        }

        configureSettings?.Invoke(settings);

        IConnectionFactory CreateConnectionFactory(IServiceProvider sp)
        {
            // ensure the log forwarder is initialized
            sp.GetRequiredService<RabbitMQEventSourceLogForwarder>().Start();

            var factory = new ConnectionFactory();

            var configurationOptionsSection = configSection.GetSection("ConnectionFactory");
            var namedConfigurationOptionsSection =
                namedConfigSection.GetSection("ConnectionFactory");
            configurationOptionsSection.Bind(factory);
            namedConfigurationOptionsSection.Bind(factory);

            // the connection string from settings should win over the one from the ConnectionFactory section
            var connectionString = settings.ConnectionString;
            if (!string.IsNullOrEmpty(connectionString))
            {
                factory.Uri = new(connectionString);
            }

            configureConnectionFactory?.Invoke(factory);

            return factory;
        }

        builder.Services.AddSingleton(CreateConnectionFactory);
        builder.Services.AddSingleton<IConnection>(sp =>
            CreateConnection(sp.GetRequiredService<IConnectionFactory>(),
                settings.MaxConnectRetryCount));

        builder.Services.AddSingleton<RabbitMQEventSourceLogForwarder>();

        if (!settings.DisableTracing)
        {
            // Note that RabbitMQ.Client v6.6 doesn't have built-in support for tracing. See https://github.com/rabbitmq/rabbitmq-dotnet-client/pull/1261

            builder.Services.AddOpenTelemetry()
                .WithTracing(traceBuilder => traceBuilder.AddSource(ActivitySourceName));
        }

        if (!settings.DisableHealthChecks)
        {
            builder.Services.AddHealthChecks()
                .Add(new HealthCheckRegistration(
                    "RabbitMQ.Client",
                    sp =>
                    {
                        try
                        {
                            // if the IConnection can't be resolved, make a health check that will fail
                            var options = new RabbitMQHealthCheckOptions
                            {
                                Connection = sp.GetRequiredService<IConnection>()
                            };
                            return new RabbitMQHealthCheck(options);
                        }
                        catch (Exception ex)
                        {
                            return new FailedHealthCheck(ex);
                        }
                    },
                    failureStatus: default,
                    tags: default));
        }
    }

    private sealed class FailedHealthCheck(Exception ex) : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new HealthCheckResult(context.Registration.FailureStatus,
                exception: ex));
        }
    }

    private static IConnection CreateConnection(IConnectionFactory factory, int retryCount)
    {
        var resiliencePipelineBuilder = new ResiliencePipelineBuilder();
        if (retryCount > 0)
        {
            resiliencePipelineBuilder.AddRetry(new RetryStrategyOptions
            {
                ShouldHandle = static args => args.Outcome is
                    { Exception: SocketException or BrokerUnreachableException }
                    ? PredicateResult.True()
                    : PredicateResult.False(),
                BackoffType = DelayBackoffType.Exponential,
                MaxRetryAttempts = retryCount,
                Delay = TimeSpan.FromSeconds(1),
            });
        }

        var resiliencePipeline = resiliencePipelineBuilder.Build();

        using var activity =
            _activitySource.StartActivity("rabbitmq connect", ActivityKind.Client);
        AddRabbitMQTags(activity, factory.Uri);

        return resiliencePipeline.Execute(static factory =>
        {
            using var connectAttemptActivity =
                _activitySource.StartActivity("rabbitmq connect attempt", ActivityKind.Client);
            AddRabbitMQTags(connectAttemptActivity, factory.Uri, "connect");

            try
            {
                return factory.CreateConnection();
            }
            catch (Exception ex)
            {
                if (connectAttemptActivity is not null)
                {
                    connectAttemptActivity.AddTag("exception.message", ex.Message);
                    // Note that "exception.stacktrace" is the full exception detail, not just the StackTrace property.
                    // See https://opentelemetry.io/docs/specs/semconv/attributes-registry/exception/
                    // and https://github.com/open-telemetry/opentelemetry-specification/pull/697#discussion_r453662519
                    connectAttemptActivity.AddTag("exception.stacktrace", ex.ToString());
                    connectAttemptActivity.AddTag("exception.type", ex.GetType().FullName);
                    connectAttemptActivity.SetStatus(ActivityStatusCode.Error);
                }

                throw;
            }
        }, factory);
    }

    private static void AddRabbitMQTags(Activity? activity, Uri address, string? operation = null)
    {
        if (activity is null)
        {
            return;
        }

        activity.AddTag("server.address", address.Host);
        activity.AddTag("server.port", address.Port);
        activity.AddTag("messaging.system", "rabbitmq");
        if (operation is not null)
        {
            activity.AddTag("messaging.operation", operation);
        }
    }
}