namespace eShop.Basket.API;

public sealed class ShoppingBasketItem
{
    private ShoppingBasketItem(Guid id, int productId, double unitPrice, int quantity)
    {
        Id = id;
        ProductId = productId;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    [ID]
    public Guid Id { get; private set; }

    public int ProductId { get; private set; }

    public double UnitPrice { get; private set; }

    public int Quantity { get; private set; }

    public void UpdateQuantity(int quantity)
    {
        Quantity = quantity;
    }

    public static ShoppingBasketItem Create(int productId, double unitPrice, int quantity)
    {
        return new ShoppingBasketItem(Guid.NewGuid(), productId, unitPrice, quantity);
    }
}