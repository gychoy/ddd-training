using eShop.Basket.API.Types;
using Microsoft.EntityFrameworkCore;

namespace eShop.Basket.API;

public sealed class ShoppingBasketService(BasketDbContext dbContext) : IShoppingBasketService
{
    public async Task<ShoppingBasket?> GetBasketByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await dbContext.Baskets
            .Include(b => b.Items)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<ShoppingBasket?> GetBasketByCustomerIdAsync(
        string customerId,
        CancellationToken cancellationToken)
    {
        return await dbContext.Baskets
            .Include(b => b.Items)
            .FirstOrDefaultAsync(b => b.CustomerId == customerId, cancellationToken);
    }

    public async Task<ShoppingBasket> AddToBasketAsync(string customerId,
        int productId,
        int quantity,
        double price,
        CancellationToken cancellationToken)
    {
        if (quantity <= 0)
        {
            throw new QuantityCannotBeNegativeError(quantity);
        }

        var basket = await dbContext.Baskets
            .Include(b => b.Items)
            .FirstOrDefaultAsync(b => b.CustomerId == customerId, cancellationToken);

        if (basket == null)
        {
            basket = ShoppingBasket.Create(customerId);
            dbContext.Baskets.Add(basket);
        }

        basket.AddItem(productId, price, quantity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return basket;
    }

    public async Task<ShoppingBasket> ChangeQuantityAsync(
        string customerId,
        Guid id,
        int quantity,
        CancellationToken cancellationToken)
    {
        if (quantity < 0)
        {
            throw new QuantityCannotBeNegativeError(quantity);
        }

        var basket = await dbContext.Baskets
            .Include(b => b.Items)
            .FirstOrDefaultAsync(b => b.CustomerId == customerId, cancellationToken);

        if (basket == null)
        {
            throw new InvalidBasketItemId(id);
        }

        var item = basket.Items.FirstOrDefault(i => i.Id == id);
        if (item == null)
        {
            throw new InvalidBasketItemId(id);
        }

        item.UpdateQuantity(quantity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return basket;
    }

    public async Task<ShoppingBasket> RemoveFromBasketAsync(
        string customerId,
        Guid id,
        CancellationToken cancellationToken)
    {
        var basket = await dbContext.Baskets
            .Include(b => b.Items)
            .FirstOrDefaultAsync(b => b.CustomerId == customerId, cancellationToken);

        if (basket == null)
        {
            throw new InvalidBasketItemId(id);
        }

        var item = basket.Items.FirstOrDefault(i => i.Id == id);
        if (item == null)
        {
            throw new InvalidBasketItemId(id);
        }

        basket.Items.Remove(item);

        await dbContext.SaveChangesAsync(cancellationToken);

        return basket;
    }

    public async Task<bool> DeleteBasketAsync(
        string customerId,
        CancellationToken cancellationToken)
    {
        var basket = await dbContext.Baskets
            .Include(b => b.Items)
            .FirstOrDefaultAsync(b => b.CustomerId == customerId, cancellationToken);

        if (basket == null)
        {
            return false;
        }

        dbContext.Baskets.Remove(basket);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}