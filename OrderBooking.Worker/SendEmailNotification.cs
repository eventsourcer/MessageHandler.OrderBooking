using MessageHandler.Runtime.AtomicProcessing;
using OrderBooking.Events;

namespace OrderBooking.Api.Commands;

public class SendEmailNotification(IEmailService emailService, ILogger<SendEmailNotification> logger) 
: IHandle<BookingStarted>
{
    private readonly IEmailService _emailService = emailService;
    private readonly ILogger<SendEmailNotification> _logger = logger;
    public async Task Handle(BookingStarted message, IHandlerContext? context = null)
    {
        _logger.LogInformation($"Received BookingStarted event {message.SourceId}.......");
        await _emailService.SendAsync($"New order placed with ID: {message.EventId}");
        _logger.LogInformation("BookingStarted email is sent....");
    }
}
