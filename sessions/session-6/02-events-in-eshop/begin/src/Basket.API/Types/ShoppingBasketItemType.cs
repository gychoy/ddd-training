namespace eShop.Basket.API.Types;

[ObjectType<ShoppingBasketItem>]
public static partial class ShoppingBasketItemType
{
    static partial void Configure(IObjectTypeDescriptor<ShoppingBasketItem> descriptor)
    {
        descriptor.Ignore(x => x.ProductId);
    }

    public static Product GetProduct([Parent] ShoppingBasketItem item)
    {
        return new Product { Id = item.ProductId };
    }
}