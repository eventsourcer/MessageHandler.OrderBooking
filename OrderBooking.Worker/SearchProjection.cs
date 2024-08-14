using MessageHandler.EventSourcing.Projections;
using OrderBooking.Events;

public class SearchProjection :
    IProjection<SalesOrder, BookingStarted>,
    IProjection<SalesOrder, SalesOrderConfirmed>
{
    public void Project(SalesOrder salesOrder, BookingStarted msg)
    {
        salesOrder.Name = msg.Name;
        salesOrder.Amount = msg.PurchaseOrder?.Amount ?? 0;
        salesOrder.Status = "Pending";
    }

    public void Project(SalesOrder salesOrder, SalesOrderConfirmed msg)
    {
        salesOrder.Status = "Confirmed";
    }
}