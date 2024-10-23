using System.Collections.Concurrent;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eShop.Catalog.Application.Common.Behaviours;

public sealed class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        logger.HandleCommand(request.GetGenericTypeName());

        try
        {
            var response = await next();

            logger.CommandHandled(request.GetGenericTypeName());

            return response;
        }
        catch (Exception ex)
        {
            logger.CommandFailed(request.GetGenericTypeName(), ex);
            throw;
        }
    }
}

file static class Extensions
{
    private static readonly ConcurrentDictionary<Type, string> TypeNames = new();

    public static string GetGenericTypeName<T>(this T _) =>
        TypeNames.GetOrAdd(typeof(T), _ => BuildTypeName(typeof(T)));

    private static string BuildTypeName(Type type)
    {
        var typeName = type.Name;

        if (type.IsGenericType)
        {
            var arguments = string.Join(", ", type.GetGenericArguments().Select(BuildTypeName));
            return $"{typeName}<{arguments}>";
        }

        return typeName;
    }
}

internal static partial class Logs
{
    [LoggerMessage(LogLevel.Information, "Handling command {CommandName}")]
    public static partial void HandleCommand(this ILogger logger, string commandName);

    [LoggerMessage(LogLevel.Information, "Command {CommandName} handled")]
    public static partial void CommandHandled(this ILogger logger, string commandName);

    [LoggerMessage(LogLevel.Error, "Command {CommandName} failed")]
    public static partial void CommandFailed(this ILogger logger, string commandName, Exception ex);
}