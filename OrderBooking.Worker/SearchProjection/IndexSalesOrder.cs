using Azure.Search.Documents;
using MessageHandler.EventSourcing.Projections;
using MessageHandler.Runtime.AtomicProcessing;
using OrderBooking.Events;

public class IndexSalesOrder : IHandle<BookingStarted>
{
    private readonly SearchClient _client;
    private readonly IInvokeProjections<SalesOrder> _projection;
    private readonly ILogger<IndexSalesOrder> _logger;

    public IndexSalesOrder(IInvokeProjections<SalesOrder> projection, SearchClient client, ILogger<IndexSalesOrder> logger = null!)
    {
        _client = client;
        _projection = projection;
        _logger = logger;
    }

    public async Task Handle(BookingStarted message, IHandlerContext context)
    {
        _logger?.LogInformation($"{DateTime.Now}: Received BookingStarted, indexing the sales order...");

        var salesOrder = await _client.GetOrCreateSalesOrder(message.BookingId);

        _projection.Invoke(salesOrder, message);

        await _client.Persist(salesOrder);

        _logger?.LogInformation("Sales order indexed");
    }
}