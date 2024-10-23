namespace eShop.Basket.API.Types;

public sealed class QuantityCannotBeNegativeError(int quantity)
    : Exception($"Quantity cannot be negative: {quantity}");