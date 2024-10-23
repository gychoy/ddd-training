using eShop.Catalog.Common;
using eShop.Catalog.Entities.Products;

namespace eShop.Catalog.Events;

public record ProductPriceChangedEvent(Product Product, decimal NewPrice) : Event;
