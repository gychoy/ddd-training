namespace eShop.Catalog.Application.Common.Contracts;

public interface IDataTransaction : IAsyncDisposable
{
    Guid Id { get; }

    Task CommitAsync(CancellationToken cancellationToken = default);

    Task RollbackAsync(CancellationToken cancellationToken = default);
}
