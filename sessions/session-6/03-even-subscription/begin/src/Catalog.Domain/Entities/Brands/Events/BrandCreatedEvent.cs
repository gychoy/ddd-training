using eShop.Catalog.Common;
using eShop.Catalog.Entities.Brands;

namespace eShop.Catalog.Events;

public sealed record BrandCreatedEvent(Brand Brand) : Event;
