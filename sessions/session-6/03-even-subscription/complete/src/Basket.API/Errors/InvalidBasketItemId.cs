namespace eShop.Basket.API.Types;

public sealed class InvalidBasketItemId(Guid id) : Exception($"Invalid basket item id: {id}");