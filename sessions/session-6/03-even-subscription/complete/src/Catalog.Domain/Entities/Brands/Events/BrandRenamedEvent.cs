using eShop.Catalog.Common;
using eShop.Catalog.Entities.Brands;

namespace eShop.Catalog.Events;

public sealed record BrandRenamedEvent(Brand Brand, string NewName) : Event;
