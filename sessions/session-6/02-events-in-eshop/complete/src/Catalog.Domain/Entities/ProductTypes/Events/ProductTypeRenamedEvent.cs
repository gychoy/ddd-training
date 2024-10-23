using eShop.Catalog.Common;
using eShop.Catalog.Entities.ProductTypes;

namespace eShop.Catalog.Events;

public sealed record ProductTypeRenamedEvent(ProductType ProductType, string NewName) : Event;