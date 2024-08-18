using OrderBooking.Events;

namespace OrderBooking.Api.Commands;

public record PlacePurchaseOrder(string BuyerId, string Name, PurchaseOrder PurchaseOrder);