using System.Collections.Concurrent;
using MessageHandler.EventSourcing;
using MessageHandler.EventSourcing.AzureTableStorage;
using MessageHandler.EventSourcing.DomainModel;
using MessageHandler.EventSourcing.Projections;
using MessageHandler.Runtime;
using MessageHandler.Runtime.ConfigurationSettings;
using MessageHandler.Runtime.Licensing;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualBasic;
using OrderBooking;
using OrderBooking.Api.Commands;
using OrderBooking.Projections;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// var eventSourcingConfig = new EventsourcingRuntime();
// var runtimeConfig = new HandlerRuntimeConfiguration();
// var handlerRuntime = new MessageHandler.Runtime.HandlerRuntime();
// // var root = new MessageHandler.Runtime.ConfigurationRoot();

// class ConfRoot : EventsourcingRuntime
// {
//     void Root()
//     {
//         var rt = HandlerName;
//     }
// }
// class SettingsRoott : ProjectionsRestorer
// {
//     void Root()
//     {
//         settings
//     }
// }
//  class MyRUnTime : TokenContent
// {
//     void D()
//     {
//         var tr = base.GetLicensingToken;
//     }
// }



builder.Services.AddMessageHandler(nameof(OrderBookingAggregate), runtimeConfiguration =>
{
    runtimeConfiguration.License(builder.Configuration["LicenseToken"]);
    var connectionString = builder.Configuration.Get<string>("TableStorageConnection")
                                   ?? throw new Exception("No 'TableStorageConnection' was provided. Use User Secrets or specify via environment variable.");

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



// builder.Services.AddCors(options =>
// {
//     options.AddPolicy("AllowAll", policy =>
//     {
//         policy.AllowAnyMethod()
//               .AllowAnyOrigin()
//               .AllowAnyHeader();
//     });
// });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
// app.UseCors("AllowAll");


app.MapPost("api/orderbooking/{bookingId}", 
async (
    IEventSourcedRepository<OrderBookingAggregate> repo,
    PlacePurchaseOrder command,
    string bookingId
    ) =>
{
    var booking = await repo.Get(bookingId);
    booking.PlacePurchaseOrder(command.PurchaseOrder);

    await repo.Flush();

    return Results.Ok(booking.Id);
});

app.MapGet("api/orderbooking", async(IRestoreProjections<Booking> projector, string id) =>
    Results.Ok(await projector.Restore(nameof(OrderBookingAggregate), id))
);
app.MapGet("api/orderbookingdetailed", async(IRestoreProjections<BookingDetail> projector, string id) =>
    Results.Ok(await projector.Restore(nameof(OrderBookingAggregate), id))
);

app.MapGet("test", async(IServiceProvider serviceProvider, IConfiguration conf) =>
{
    var confs = serviceProvider.GetServices(conf.GetType());
    var manager = new ConfigurationManager();
    // var c = new Conf();
    await Conf.Go(serviceProvider);
});

app.Run();

static class Conf
{
    internal static string GetLicensingToken(this MessageHandler.Runtime.ConfigurationRoot configuration)
    {
        return "messagehandler.licensetoken";
    }
    public static string GetValue<T>(this IConfiguration configuration, string key)
    {
        return configuration.GetSection(key).Get<string>();
    }
    public static string Bind(this IConfiguration configuration, object? key)
    {
        return configuration.GetSection(key.ToString()).Get<string>();
    }
    public static string Bind(this IConfiguration configuration, object? key, Action<BinderOptions>? options)
    {
        return configuration.GetSection(key.ToString()).Get<string>();
    }
    public static string Bind(this IConfiguration configuration, string key, object? binder)
    {
        return configuration.GetSection(key.ToString()).Get<string>();
    }
    public static string GetValue(this IConfiguration configuration, Type type, string key)
    {
        return configuration.GetSection(key).Get<string>();
    }
    public static string Get<T>(this IConfiguration configuration, string key)
    {
        return configuration.GetSection(key).Get<string>();
    }
    public static string Get(this IConfiguration configuration, Type type, string key)
    {
        return configuration.GetSection(key).Get<string>();
    }
    public static string GetSection(this IConfiguration configuration, string key)
    {
        return "configuration.GetSection(key).Get<string>();";
    }
    public static async Task Go(IServiceProvider serviceProvider)
    {
        var service = serviceProvider.GetServices<IHostedService>().First(x => x.GetType().Name == "CheckLicense");
        
        await service.StartAsync(new CancellationToken());
    }
}
