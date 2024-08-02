using MessageHandler.EventSourcing.Contracts;
using MessageHandler.EventSourcing.Projections;
using MessageHandler.EventSourcing.Testing;
using OrderBooking.Events;
using OrderBooking.Projections;

namespace OrderBooking.UnitTests;

public class ProjectionTests
{
    [Fact]
    public void GivenBookingStarted_ShouldSetStatusStarted()
    {
        // Given
        SourcedEvent[] history =
        [
            new BookingStarted("1", new PurchaseOrder("")),
        ];
        var booking = new Booking();
    
        // When
        var invoker = new TestProjectionInvoker<Booking>(new BookingProjection());
        invoker.Invoke(booking, history);
    
        // Then
        Assert.Equal(BookingStatus.Pending, booking.Status);
    }
    [Fact]
    public void GivenBookingConfirmed_ShouldSetStatusConfirmed()
    {
        // Given
        SourcedEvent[] history =
        [
            new BookingStarted("1", new PurchaseOrder("")),
            new SalesOrderConfirmed("", new PurchaseOrder(""))
        ];
    
        // When
        var booking = new Booking();
        var invoker = new TestProjectionInvoker<Booking>(new BookingProjection());
        invoker.Invoke(booking, history);
    
        // Then
        Assert.Equal(BookingStatus.Confirmed, booking.Status);
    }
}