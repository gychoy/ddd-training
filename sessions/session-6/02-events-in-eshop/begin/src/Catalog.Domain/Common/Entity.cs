namespace eShop.Catalog.Common;

/// <summary>
/// Base class for entities
/// </summary>
public abstract class Entity
{
    public EventCollection Events { get; } = [];
}