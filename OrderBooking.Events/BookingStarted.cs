using MessageHandler.EventSourcing.Contracts;

namespace OrderBooking.Events;

public class BookingStarted(string bookingId, PurchaseOrder purchaseOrder) : SourcedEvent
{
    public string BookingId => bookingId;
    public string Name => purchaseOrder.Name;
    public PurchaseOrder PurchaseOrder => purchaseOrder;
}