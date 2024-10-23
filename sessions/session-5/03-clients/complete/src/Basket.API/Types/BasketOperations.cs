namespace eShop.Basket.API.Types;

public static class BasketOperations
{
    [Query]
    public static Viewer Me() => new();

    [Query]
    [NodeResolver]
    public static Task<ShoppingBasket?> GetBasketByIdAsync(
        Guid id,
        IShoppingBasketService basketService,
        CancellationToken ct)
    {
        return basketService.GetBasketByIdAsync(id, ct);
    }

    [Mutation]
    [Error<QuantityCannotBeNegativeError>]
    public static async Task<ShoppingBasket> AddToBasketAsync(
        [ID<Product>] int productId,
        int quantity,
        double price,
        [GlobalState("customerId")] string customerId,
        IShoppingBasketService basketService,
        CancellationToken ct)
    {
        return await basketService.AddToBasketAsync(customerId, productId, quantity, price, ct);
    }

    [Mutation]
    [Error<QuantityCannotBeNegativeError>]
    [Error<InvalidBasketItemId>]
    public static async Task<ShoppingBasket> ChangeQuantityAsync(
        [ID<ShoppingBasket>] Guid id,
        int quantity,
        [GlobalState("customerId")] string customerId,
        IShoppingBasketService basketService,
        CancellationToken ct)
    {
        return await basketService.ChangeQuantityAsync(customerId, id, quantity, ct);
    }

    [Mutation]
    [Error<InvalidBasketItemId>]
    public static async Task<ShoppingBasket> RemoveFromBasketAsync(
        [ID<ShoppingBasket>] Guid id,
        [GlobalState("customerId")] string customerId,
        IShoppingBasketService basketService,
        CancellationToken ct)
    {
        return await basketService.RemoveFromBasketAsync(customerId, id, ct);
    }

    [Mutation]
    [UseMutationConvention(PayloadFieldName = "deleted")]
    public static async Task<bool> ClearBasketAsync(
        [GlobalState("customerId")] string customerId,
        IShoppingBasketService basketService,
        CancellationToken ct)
    {
        var deleted = await basketService.DeleteBasketAsync(customerId, ct);

        return deleted;
    }
}