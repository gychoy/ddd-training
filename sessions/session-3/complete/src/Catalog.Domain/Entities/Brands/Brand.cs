﻿// ReSharper disable CollectionNeverUpdated.Global

using System.ComponentModel.DataAnnotations;

namespace eShop.Catalog.Entities.Brands;

public sealed class Brand
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
    }

    public static Brand Create(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var brand = new Brand { Name = name };
        return brand;
    }
}