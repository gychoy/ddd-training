namespace eShop.Catalog.Application.Common.Contracts;

public interface IDataExecutionStrategy
{
    Task<T> ExecuteAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken = default);
}