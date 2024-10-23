using eShop.Catalog.Common;

namespace eShop.Catalog.Application.Common.Contracts;

public interface ICatalogDataContext : IUnitOfWork
{
    bool HasActiveTransaction { get; }

    IDataExecutionStrategy CreateExecutionStrategy();

    Task<IDataTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
