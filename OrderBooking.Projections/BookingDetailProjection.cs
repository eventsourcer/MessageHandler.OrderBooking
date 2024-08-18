using MessageHandler.EventSourcing.Projections;
using OrderBooking.Events;

namespace OrderBooking.Projections;

public class BookingDetailProjection : 
    IProjection<BookingDetail, BookingStarted>,
    IProjection<BookingDetail, SalesOrderConfirmed>
{
    public void Project(BookingDetail booking, BookingStarted bookingStarted)
    {
        booking.SourceId = bookingStarted.BookingId;
        booking.Status = BookingStatus.Pending;
    }
    public void Project(BookingDetail booking, SalesOrderConfirmed salesOrderConfirmed)
    {
        booking.Status = BookingStatus.Confirmed;
    }
}