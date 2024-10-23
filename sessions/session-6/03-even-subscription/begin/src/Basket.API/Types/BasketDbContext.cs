using Microsoft.EntityFrameworkCore;

namespace eShop.Basket.API;

public sealed class BasketDbContext(DbContextOptions<BasketDbContext> options) : DbContext(options)
{
    public DbSet<ShoppingBasket> Baskets { get; set; }

    public DbSet<ShoppingBasketItem> BasketItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ShoppingBasket>().HasKey(b => b.Id);

        modelBuilder
            .Entity<ShoppingBasket>()
            .HasMany(b => b.Items)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ShoppingBasketItem>().HasKey(i => i.Id);
    }
}

