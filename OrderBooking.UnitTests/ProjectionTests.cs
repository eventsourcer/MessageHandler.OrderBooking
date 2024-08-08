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
            new BookingStarted("1", new PurchaseOrder("", 1)),
        ];
        var booking = new Booking();
    
        // When
        var invoker = new TestProjectionInvoker<Booking>(new BookingProjection());
        invoker.Invoke(booking, history);
    
        // Then
        Assert.Equal(nameof(BookingStatus.Pending), booking.Status);
    }
    [Fact]
    public void GivenBookingConfirmed_ShouldSetStatusConfirmed()
    {
        // Given
        SourcedEvent[] history =
        [
            new BookingStarted("1", new PurchaseOrder("", 1)),
            new SalesOrderConfirmed("", new PurchaseOrder("", 1))
        ];
    
        // When
        var booking = new Booking();
        var invoker = new TestProjectionInvoker<Booking>(new BookingProjection());
        invoker.Invoke(booking, history);
    
        // Then
        Assert.Equal(nameof(BookingStatus.Confirmed), booking.Status);
    }
}