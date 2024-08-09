using Microsoft.AspNetCore.SignalR;

namespace OrderBooking.Api.Services;

public class EventsHub : Hub
{
    public async Task Subscribe()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "all");
    }
}