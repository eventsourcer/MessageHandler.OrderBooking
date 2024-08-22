using MessageHandler.EventSourcing.Projections;
using MessageHandler.Runtime.AtomicProcessing;
using OrderBooking.Events;

public class IndexConfirmationMail : IHandle<BookingStarted>
{
    private readonly IPersistAvailableConfirmationMails _client;
    private readonly IInvokeProjections<ConfirmationMail> _projection;
    private readonly ILogger<IndexConfirmationMail> _logger;

    public IndexConfirmationMail(IInvokeProjections<ConfirmationMail> projection, IPersistAvailableConfirmationMails client, ILogger<IndexConfirmationMail> logger = null!)
    {
        _client = client;
        _projection = projection;
        _logger = logger;
    }

    public async Task Handle(BookingStarted message, IHandlerContext context)
    {
        _logger?.LogInformation($"{DateTime.Now}: Received BookingStarted, indexing the confirmation mail...");

        var confirmationMail = await _client.GetConfirmationMail(message.BookingId);

        _projection.Invoke(confirmationMail, message);

        await _client.Insert(confirmationMail);

        _logger?.LogInformation("Confirmation mail indexed");
    }
}