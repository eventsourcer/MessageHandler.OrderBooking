using MessageHandler.EventSourcing.DomainModel;
using OrderBooking.Events;

namespace OrderBooking;

public class OrderBookingAggregate : EventSourced, IApply<BookingStarted>, IApply<SalesOrderConfirmed>
{
    // private BookingStatus BookingStatus { get; set; }
    private bool _alreadyStarted;
    private bool _confirmed;
    public OrderBookingAggregate() : base(Guid.NewGuid().ToString())
    {
        
    }
    public OrderBookingAggregate(string id) : base(id)
    {
        
    }
    public void Apply(BookingStarted bookingStarted)
    {
        // BookingStatus = BookingStatus.Pending;
        _alreadyStarted = true;
    }
    public void Apply(SalesOrderConfirmed orderConfirmed)
    {
        // BookingStatus = BookingStatus.Confirmed;
        _confirmed = true;
    }

    public void PlacePurchaseOrder(string buyerId, string name, PurchaseOrder purchaseOrder)
    {
        if(_alreadyStarted) return;
        
        Emit(new BookingStarted(Id, buyerId, name, purchaseOrder));
    }
    public void ConfirmSalesOrder()
    {
        if(_confirmed) return;

        Emit(new SalesOrderConfirmed(Id));
    }
}
