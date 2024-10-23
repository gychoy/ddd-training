namespace eShop.Basket.API;

public interface IShoppingBasketService
{
    Task<ShoppingBasket?> GetBasketByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<ShoppingBasket?> GetBasketByCustomerIdAsync(
        string customerId,
        CancellationToken cancellationToken);

    Task<ShoppingBasket> AddToBasketAsync(
        string customerId,
        int productId,
        int quantity,
        double price,
        CancellationToken cancellationToken);

    Task<ShoppingBasket> ChangeQuantityAsync(
        string customerId,
        Guid id,
        int quantity,
        CancellationToken cancellationToken);

    Task<ShoppingBasket> RemoveFromBasketAsync(
        string customerId,
        Guid id,
        CancellationToken cancellationToken);

    Task<bool> DeleteBasketAsync(string customerId, CancellationToken cancellationToken);
}