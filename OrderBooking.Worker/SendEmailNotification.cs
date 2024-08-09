using MessageHandler.Runtime.AtomicProcessing;
using OrderBooking.Events;

namespace OrderBooking.Api.Commands;

public class SendEmailNotification(IEmailService emailService) : IHandle<BookingStarted>
{
    private readonly IEmailService _emailService = emailService;
    public async Task Handle(BookingStarted message, IHandlerContext? context = null)
    {
        await _emailService.SendAsync($"New order placed with ID: {message.EventId}");
    }
}
