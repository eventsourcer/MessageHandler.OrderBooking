using MessageHandler.EventSourcing.Contracts;
using OrderBooking.Events;

namespace OrderBooking.UnitTests;

public class AggregateRootTests
{
    [Fact]
    public void GivenValidPurchaseOrder_OrderShouldBeStarted()
    {
        // given
        var purchaseOrder = new PurchaseOrder("sarwan");

        // when
        var orderBooking = new OrderBookingAggregate();
        orderBooking.PlacePurchaseOrder(purchaseOrder);
        var pendingeEvents = orderBooking.Commit();

        // then
        Func<SourcedEvent, bool> check = @event => typeof(BookingStarted).IsAssignableFrom(@event.GetType());
        var bookingStarted = pendingeEvents.FirstOrDefault(check);

        Assert.NotNull(bookingStarted);
    }
    [Fact]
    public void GivenValidPurchaseOrder_WhenPlacingBookingTwice_ShouldStartOnlyOnce()
    {
        // Given
        var purchaseOrder = new PurchaseOrder("sarwan");
    
        // When
        var aggregate = new OrderBookingAggregate();
        aggregate.PlacePurchaseOrder(purchaseOrder);
        aggregate.PlacePurchaseOrder(purchaseOrder);
        var pendingEvents = aggregate.Commit();
    
        // Then
        Assert.Single(pendingEvents.Where(x => x.GetType() == typeof(BookingStarted)));
    }
    [Fact]
    public void GivenValidPurchaseOrder_PurchaseOrderIsCarriedOverEvent()
    {
        // Given
        var purchaseOrder = new PurchaseOrder("sarwan");
    
        // When
        var aggregate = new OrderBookingAggregate();
        aggregate.PlacePurchaseOrder(purchaseOrder);
        var pendingEvents = aggregate.Commit();
    
        // Then
        var bookingStarted = pendingEvents.First() as BookingStarted;
        Assert.NotNull(bookingStarted?.BookingId);
        Assert.NotNull(bookingStarted.Name);
        Assert.NotNull(bookingStarted.PurchaseOrder);
        
    }
}