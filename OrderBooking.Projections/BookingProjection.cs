using MessageHandler.EventSourcing.Projections;
using OrderBooking.Events;

namespace OrderBooking.Projections;

public class BookingProjection : 
    IProjection<Booking, BookingStarted>,
    IProjection<BookingDetail, BookingStarted>,
    IProjection<Booking, SalesOrderConfirmed>
{
    public void Project(Booking booking, BookingStarted bookingStarted)
    {
        booking.Status = nameof(BookingStatus.Pending);
    }
    public void Project(BookingDetail booking, BookingStarted bookingStarted)
    {
        booking.SourceId = bookingStarted.SourceId;
        booking.Status = BookingStatus.Pending;
    }
    public void Project(Booking booking, SalesOrderConfirmed salesOrderConfirmed)
    {
        booking.Status = Enum.GetName(BookingStatus.Confirmed);
    }
}