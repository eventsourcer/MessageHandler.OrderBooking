using OrderBooking.Events;

namespace OrderBooking.Api.Commands;

public record PlacePurchaseOrder(string Name, int Amount);