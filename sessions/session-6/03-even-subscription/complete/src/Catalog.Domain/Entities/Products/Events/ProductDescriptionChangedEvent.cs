using eShop.Catalog.Common;
using eShop.Catalog.Entities.Products;

namespace eShop.Catalog.Events;

public record ProductDescriptionChangedEvent(Product Product, string NewDescription) : Event;
