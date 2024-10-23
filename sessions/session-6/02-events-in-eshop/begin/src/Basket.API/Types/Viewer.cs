namespace eShop.Basket.API.Types;

public sealed class Viewer
{
    public Task<ShoppingBasket?> GetBasketAsync(
        [GlobalState("customerId")] string customerId,
        IShoppingBasketService basketService,
        CancellationToken ct)
    {
        return basketService.GetBasketByCustomerIdAsync(customerId, ct);
    }
}