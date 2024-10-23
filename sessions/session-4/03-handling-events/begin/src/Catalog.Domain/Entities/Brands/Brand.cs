﻿// ReSharper disable CollectionNeverUpdated.Global

using System.ComponentModel.DataAnnotations;
using eShop.Catalog.Common;
using eShop.Catalog.Events;

namespace eShop.Catalog.Entities.Brands;

public sealed class Brand : Entity
{
    private Brand()
    {
    }

    public int Id { get; private set; }

    [Required]
    public string Name { get; private set; } = default!;

    public void Rename(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
        Events.Add(new BrandRenamedEvent(this, name));
    }

    public static Brand Create(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var brand = new Brand { Name = name };
        brand.Events.Add(new BrandCreatedEvent(brand));
        return brand;
    }
}