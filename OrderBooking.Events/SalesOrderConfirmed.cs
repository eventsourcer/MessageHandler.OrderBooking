using MessageHandler.EventSourcing.Contracts;

namespace OrderBooking.Events;

public class SalesOrderConfirmed(string id, PurchaseOrder purchaseOrder) : SourcedEvent
{
    public string Id => id;
    public PurchaseOrder PurchaseOrder => purchaseOrder;
}