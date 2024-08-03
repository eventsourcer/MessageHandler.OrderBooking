using MessageHandler.EventSourcing;
using MessageHandler.EventSourcing.AzureTableStorage;
using MessageHandler.Runtime;
using MessageHandler.Runtime.Licensing;
using Microsoft.Extensions.Azure;
using OrderBooking;
using OrderBooking.Projections;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// var eventSourcingConfig = new EventsourcingConfiguration();
// var runtimeConfig = new HandlerRuntimeConfiguration();
// var handlerRuntime = new MessageHandler.Runtime.HandlerRuntime();
// var root = new MessageHandler.Runtime.ConfigurationRoot();


builder.Services.AddMessageHandler("orderbooking", runtimeConfiguration =>
{
    var connectionString = builder.Configuration.GetValue<string>("TableStorageConnectionString")
                                   ?? throw new Exception("No 'TableStorageConnectionString' was provided. Use User Secrets or specify via environment variable.");

    runtimeConfiguration.EventSourcing(source =>
    {
        source.Stream(nameof(OrderBooking),
            from => from.AzureTableStorage(connectionString, nameof(OrderBooking)),
            into =>
            {
                into.Aggregate<OrderBookingAggregate>();
                into.Projection<BookingProjection>();
            });
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


app.Run();
