using Azure.Core.Pipeline;
using Azure.Search.Documents;
using MessageHandler.EventSourcing;
using MessageHandler.EventSourcing.AzureTableStorage;
using MessageHandler.EventSourcing.DomainModel;
using MessageHandler.EventSourcing.Outbox;
using MessageHandler.EventSourcing.Projections;
using MessageHandler.Runtime;
using MessageHandler.Runtime.AtomicProcessing;
using MessageHandler.Runtime.Licensing;
using Microsoft.AspNetCore.Http.HttpResults;
using NotificationPreferences;
using OrderBooking;
using OrderBooking.Api;
using OrderBooking.Api.Commands;
using OrderBooking.Api.Models;
using OrderBooking.Api.Services;
using OrderBooking.Events;
using OrderBooking.Projections;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
 
builder.Services.AddMessageHandler(nameof(OrderBookingAggregate), runtimeConfiguration =>
{
    var connectionString = builder.Configuration.GetValue<string>("TableStorageConnection")
                                   ?? throw new Exception("No 'TableStorageConnection' was provided. Use User Secrets or specify via environment variable.");

    runtimeConfiguration.License(builder.Configuration["LicenseToken"]);

    runtimeConfiguration.EventSourcing(source =>
    {
        source.Stream(nameof(OrderBookingAggregate),
            from => from.AzureTableStorage(connectionString, nameof(OrderBookingAggregate)),
            into =>
            {
                into.Aggregate<OrderBookingAggregate>()
                .EnableTransientChannel<NotifySeller>()
                .EnableOutbox(nameof(OrderBookingAggregate), nameof(OrderBooking.Api), pipeline =>
                    pipeline.RouteMessages(to => to.Topic("asynchandler-topic", builder.Configuration.GetValue<string>("ServiceBusConnection")))
                );
                into.Projection<BookingProjection>();
                // into.Projection<BookingDetailProjection>();
            });
        source.Stream(nameof(NotificationAggregate),
        from => from.AzureTableStorage(connectionString, nameof(NotificationAggregate)),
        into =>
        {
            into.Aggregate<NotificationAggregate>()
                .EnableOutbox(nameof(NotificationAggregate), nameof(OrderBooking.Api), pipeline =>
                {
                    pipeline.RouteMessages(to => to.Topic("asynchandler-notifications-topic", builder.Configuration["ServiceBusConnection"]));
                });
        });
    });
});

builder.Services.AddSignalR();

builder.Services.AddSearchConfiguration(builder.Configuration["SearchEndpoint"] ?? "", builder.Configuration["SearchApiKey"] ?? "");

builder.Services.AddCorsConfguration(config =>
{
    config.AddPolicy("all", policy =>
    {
        policy.AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
        .SetIsOriginAllowed(hostName => true);
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("all");

app.MapHub<EventsHub>("/events");

app.MapPost("api/orderbooking/{id}", 
async (
    IEventSourcedRepository<OrderBookingAggregate> repo,
    string id,
    PlacePurchaseOrder command
    ) =>
{
    var booking = await repo.Get(id);
    booking.PlacePurchaseOrder(command.BuyerId, command.Name, command.PurchaseOrder);

    await repo.Flush();

    return Results.Ok(booking.Id);
});

app.MapGet("api/orderbooking/{id}", async(IRestoreProjections<Booking> projector, string id) =>
    Results.Ok(await projector.Restore(nameof(OrderBookingAggregate), id))
);
app.MapGet("api/orderbookingdetailed/{id}", async(IRestoreProjections<BookingDetail> projector, string id) =>
    Results.Ok(await projector.Restore(nameof(OrderBookingAggregate), id))
);
app.MapGet("api/pendingOrders", async(SearchClient client) =>
{
    var response = await client.SearchAsync<SalesOrder>("*");
    var pendingOrders = response.Value.GetResults().Select(x => x.Document);

    return TypedResults.Ok(pendingOrders);
});
app.MapPost("api/{bookingId}/confirm",
async (IEventSourcedRepository<OrderBookingAggregate> repo, string bookingId) =>
{
    var aggregate = await repo.Get(bookingId);
    aggregate.ConfirmSalesOrder();
    await repo.Flush();

    return Results.Ok(aggregate.Id);
}
);

app.MapPost("api/notifications",
async(IEventSourcedRepository<NotificationAggregate> repo, SetConfirmationMail command) =>
{
    var aggregate = await repo.Get(command.BuyerId);
    aggregate.SetConfirmationEmail(command.EmailAddress);
    await repo.Flush();

    return Results.Ok(aggregate.Id);
});

app.Run();