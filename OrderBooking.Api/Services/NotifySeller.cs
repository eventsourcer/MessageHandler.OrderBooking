using MessageHandler.EventSourcing.Contracts;
using MessageHandler.EventSourcing.DomainModel;
using Microsoft.AspNetCore.SignalR;

namespace OrderBooking.Api.Services;

public class NotifySeller(IHubContext<EventsHub> hubContext) : IChannel
{
    private readonly IHubContext<EventsHub> _hubContext = hubContext;
    public Task Push(IEnumerable<SourcedEvent> pendingEvents)
    {
        List<Task> tasks = []; 
        foreach (var item in pendingEvents)
        {
            tasks.Add(_hubContext.Clients.Group("all").SendAsync("Notify", item));
        }
        return Task.WhenAll(tasks);
    }
}
