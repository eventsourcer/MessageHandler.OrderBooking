using MessageHandler.EventSourcing.DomainModel;
using OrderBooking.Events;

namespace OrderBooking;

public class OrderBookingAggregate : EventSourced, IApply<BookingStarted>
{
    private bool _alreadyStarted;
    public OrderBookingAggregate() : base(Guid.NewGuid().ToString())
    {
        
    }
    public OrderBookingAggregate(string id) : base(id)
    {
        
    }
    public void Apply(BookingStarted evt)
    {
        _alreadyStarted = true;
    }

    public void PlacePurchaseOrder(PurchaseOrder purchaseOrder)
    {
        if(_alreadyStarted) return;
        
        Emit(new BookingStarted(Id, purchaseOrder));
    }
}
