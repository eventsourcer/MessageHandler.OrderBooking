using MessageHandler.EventSourcing.Projections;
using OrderBooking.Events;

namespace OrderBooking.Projections;

public class BookingProjection : 
    IProjection<Booking, BookingStarted>,
    IProjection<Booking, SalesOrderConfirmed>
{
    public void Project(Booking booking, BookingStarted bookingStarted)
    {
        booking.Status = BookingStatus.Pending;
    }
    public void Project(Booking booking, SalesOrderConfirmed salesOrderConfirmed)
    {
        booking.Status = BookingStatus.Confirmed;
    }
}