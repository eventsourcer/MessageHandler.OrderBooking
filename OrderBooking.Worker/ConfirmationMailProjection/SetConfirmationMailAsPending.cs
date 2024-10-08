using MessageHandler.EventSourcing.Projections;
using MessageHandler.Runtime.AtomicProcessing;
using OrderBooking.Events;

public class SetConfirmationMailAsPending : IHandle<SalesOrderConfirmed>
{
    private readonly IPersistAvailableConfirmationMails _client;
    private readonly IInvokeProjections<ConfirmationMail> _projection;
    private readonly ILogger<IndexConfirmationMail> _logger;

    public SetConfirmationMailAsPending(IInvokeProjections<ConfirmationMail> projection, IPersistAvailableConfirmationMails client, ILogger<IndexConfirmationMail> logger = null!)
    {
        _client = client;
        _projection = projection;
        _logger = logger;
    }

    public async Task Handle(SalesOrderConfirmed message, IHandlerContext context)
    {
        _logger?.LogInformation("Received SalesOrderConfirmed, marking the confirmation mail as pending...");

        var confirmationMail = await _client.GetConfirmationMail(message.BookingId);

        _projection.Invoke(confirmationMail, message);

        await _client.Update(confirmationMail);

        _logger?.LogInformation("Sales order marked as pending");
    }
}