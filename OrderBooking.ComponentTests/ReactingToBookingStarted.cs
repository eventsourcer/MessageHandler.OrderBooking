using MessageHandler.EventSourcing.Contracts;
using Microsoft.AspNetCore.SignalR;
using Moq;
using OrderBooking.Api.Commands;
using OrderBooking.Api.Services;
using OrderBooking.Events;

namespace OrderBooking.ComponentTests;

public class ReactingToBookingStarted
{
    private readonly Mock<IClientProxy> mockClientProxy = new();
    private readonly Mock<IHubClients> mockHubClients = new();
    private readonly Mock<IHubContext<EventsHub>> mockHubContext = new();
    private readonly Mock<IEmailService> mockEmailService = new();

    public ReactingToBookingStarted()
    {
        mockClientProxy.Setup(x => x.SendCoreAsync("Notify", new object?[0], It.IsAny<CancellationToken>()));
        mockHubClients.Setup(x => x.Group("all")).Returns(mockClientProxy.Object).Verifiable();
        mockHubContext.Setup(x => x.Clients).Returns(mockHubClients.Object).Verifiable();

        mockEmailService.Setup(x => x.SendAsync(It.IsAny<string>())).Returns(Task.CompletedTask).Verifiable();
    }
    [Fact]
    public async void GivenBookingStarted_ShouldNotifyTheSeller()
    {
        // Given
        var bookingStarted = new BookingStarted("1", new PurchaseOrder("sarwan", 1));

        // When
        var reaction = new NotifySeller(mockHubContext.Object);
        await reaction.Push([bookingStarted]);

        // Then
        mockClientProxy.Verify();
        mockHubClients.Verify();
        mockHubContext.Verify();
    }
    [Fact]
    public async Task GivenBookingStarted_ShouldSendEmailToSeller()
    {
        // Given
        var bookingStarted = new BookingStarted("", new PurchaseOrder("", 1));
    
        // When
        var reaction = new SendEmailNotification(mockEmailService.Object);
        await reaction.Handle(bookingStarted, null);
    
        // Then
        mockEmailService.Verify();
        mockEmailService.Verify(x => x.SendAsync(It.IsAny<string>()), Times.Once());
    }
}