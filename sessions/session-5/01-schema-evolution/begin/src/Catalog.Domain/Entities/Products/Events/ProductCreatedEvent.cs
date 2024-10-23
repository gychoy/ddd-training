using eShop.Catalog.Common;
using eShop.Catalog.Entities.Products;

namespace eShop.Catalog.Events;

public record ProductCreatedEvent(Product Product) : Event;
