using eShop.Catalog.Application.Common.Contracts;
using MediatR;

namespace eShop.Catalog.Application.Common.Behaviours;

public sealed class TransactionBehavior<TRequest, TResponse>(ICatalogDataContext context)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (context.HasActiveTransaction)
        {
            return await next();
        }

        var strategy = context.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(
            ct => ExecuteWithTransactionAsync(context, next, ct),
            cancellationToken);
    }

    private static async Task<T> ExecuteWithTransactionAsync<T>(
        ICatalogDataContext ctx,
        RequestHandlerDelegate<T> next,
        CancellationToken ct)
    {
        await using var transaction = await ctx.BeginTransactionAsync(ct);
        var response = await next();
        await transaction.CommitAsync(ct);
        return response;
    }
}