using HotChocolate.Execution.Configuration;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class CustomRequestExecutorBuilderExtensions
{
    public static IRequestExecutorBuilder AddGraphQLConventions(
        this IRequestExecutorBuilder builder)
    {
        builder.AddPagingArguments();
        builder.AddGlobalObjectIdentification();
        builder.AddMutationConventions();
        return builder;
    } 
}