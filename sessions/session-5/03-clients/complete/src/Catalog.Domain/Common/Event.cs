using MediatR;

namespace eShop.Catalog.Common;

/// <summary>
/// Base class for domain events.
/// </summary>
public abstract record Event : INotification;