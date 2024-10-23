using eShop.Catalog.Common;
using eShop.Catalog.Entities.Products;

namespace eShop.Catalog.Events;

public record ProductAddedStockEvent(Product Product, int Quantity) : Event;
