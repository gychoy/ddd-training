using eShop.Catalog.Common;
using eShop.Catalog.Entities.Products;

namespace eShop.Catalog.Events;

public record ProductRemovedStockEvent(Product Product, int Quantity) : Event;
