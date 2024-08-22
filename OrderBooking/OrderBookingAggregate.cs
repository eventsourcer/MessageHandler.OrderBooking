using MessageHandler.EventSourcing.DomainModel;
using OrderBooking.Events;

namespace OrderBooking;

public class OrderBookingAggregate : EventSourced, IApply<BookingStarted>
{
    private BookingStatus BookingStatus { get; set; }
    public OrderBookingAggregate() : base(Guid.NewGuid().ToString())
    {
        
    }
    public OrderBookingAggregate(string id) : base(id)
    {
        
    }
    public void Apply(BookingStarted bookingStarted)
    {
        BookingStatus = BookingStatus.Pending;
    }
    public void Apply(SalesOrderConfirmed orderConfirmed)
    {
        BookingStatus = BookingStatus.Confirmed;
    }

    public void PlacePurchaseOrder(string buyerId, string name, PurchaseOrder purchaseOrder)
    {
        if(BookingStatus == BookingStatus.Pending) return;
        
        Emit(new BookingStarted(Id, buyerId, name, purchaseOrder));
    }
    public void ConfirmSalesOrder()
    {
        if(BookingStatus == BookingStatus.Confirmed) return;

        Emit(new SalesOrderConfirmed(Id));
    }
    public void SetBookingAsPending(string bookingID)
    {
        Emit(new BookingChangedToPending(bookingID));
    }
}
