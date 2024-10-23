using eShop.Catalog.Application.Common.Behaviours;
using eShop.Catalog.Application.Common.Contracts;
using Microsoft.Extensions.Hosting;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class CatalogApplicationServiceCollectionExtensions
{
    public static IHostApplicationBuilder AddApplicationLayer(
        this IHostApplicationBuilder builder)
    {
        builder.Services.AddApplicationLayer();
        return builder;
    }

    internal static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddMediatR(
            c =>
            {
                c.AddOpenBehavior(typeof(LoggingBehavior<,>));
                c.AddOpenBehavior(typeof(TransactionBehavior<,>));
                c.RegisterServicesFromAssemblyContaining<ICatalogDataContext>();
                c.Lifetime = ServiceLifetime.Scoped;
            });
        return services;
    }
}