using System.Collections.Concurrent;
using MessageHandler.EventSourcing;
using MessageHandler.EventSourcing.AzureTableStorage;
using MessageHandler.EventSourcing.DomainModel;
using MessageHandler.EventSourcing.Projections;
using MessageHandler.Runtime;
using MessageHandler.Runtime.ConfigurationSettings;
using MessageHandler.Runtime.Licensing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualBasic;
using OrderBooking;
using OrderBooking.Api;
using OrderBooking.Api.Commands;
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
                into.Aggregate<OrderBookingAggregate>();
                into.Projection<BookingProjection>();
                // into.Projection<BookingDetailProjection>();
            });
    });
});

builder.Services.AddCorsConfguration(config =>
{
    config.AddPolicy("all", policy =>
    {
        policy.AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin();
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


app.MapPost("api/orderbooking/{id}", 
async (
    IEventSourcedRepository<OrderBookingAggregate> repo,
    string id,
    PlacePurchaseOrder command
    ) =>
{
    var booking = await repo.Get(id);
    booking.PlacePurchaseOrder(new PurchaseOrder(command.Name, command.Amount));

    await repo.Flush();

    return Results.Ok(booking.Id);
});

app.MapGet("api/orderbooking/{id}", async(IRestoreProjections<Booking> projector, string id) =>
    Results.Ok(await projector.Restore(nameof(OrderBookingAggregate), id))
);
app.MapGet("api/orderbookingdetailed/{id}", async(IRestoreProjections<BookingDetail> projector, string id) =>
    Results.Ok(await projector.Restore(nameof(OrderBookingAggregate), id))
);

app.Run();