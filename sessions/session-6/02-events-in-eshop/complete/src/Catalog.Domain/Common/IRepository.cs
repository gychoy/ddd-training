namespace eShop.Catalog.Common;

/// <summary>
/// Represents the repository contract.
/// </summary>
public interface IRepository
{
    /// <summary>
    /// Gets the unit of work context this repository belongs to.
    /// </summary>
    IUnitOfWork UnitOfWork { get; }
}