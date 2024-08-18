namespace OrderBooking.Events;

public record PurchaseOrder(string BuyerId, string Name, int Amount);