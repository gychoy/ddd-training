namespace eShop.Basket.API;

public sealed class ShoppingBasket
{
    private ShoppingBasket(Guid id, string customerId)
    {
        Id = id;
        CustomerId = customerId;
    }

    [ID]
    public Guid Id { get; private set; }

    public string CustomerId { get; private set; }

    public List<ShoppingBasketItem> Items { get; private set; } = [];

    public void AddItem(int productId, double unitPrice, int quantity)
    {
        var existingItem = Items.FirstOrDefault(i => i.ProductId == productId);
        if (existingItem is not null)
        {
            existingItem.UpdateQuantity(quantity);
        }
        else
        {
            Items.Add(ShoppingBasketItem.Create(productId, unitPrice, quantity));
        }
    }

    public static ShoppingBasket Create(string customerId) => Create(Guid.NewGuid(), customerId);

    public static ShoppingBasket Create(Guid id, string customerId) => new(id, customerId);
}