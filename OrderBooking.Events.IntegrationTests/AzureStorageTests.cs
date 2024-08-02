using Azure.Data.Tables;
using MessageHandler.EventSourcing;
using MessageHandler.EventSourcing.AzureTableStorage;
using MessageHandler.EventSourcing.Contracts;
using Microsoft.Extensions.Configuration;

namespace OrderBooking.Events.IntegrationTests;

public class AzureStorageTests : IAsyncLifetime
{
    private readonly string _connection;
    private readonly string _table;
    public AzureStorageTests()
    {
        var configuration = new ConfigurationBuilder()
        .AddUserSecrets<AzureStorageTests>(optional: true)
        .AddEnvironmentVariables()
        .Build();
        _connection = configuration["TableStorageConnection"] ?? "";
        _table = "t" + Guid.NewGuid().ToString("N");
    }
    public async Task InitializeAsync()
    {
        var client = new TableServiceClient(_connection);
        var table = client.GetTableClient(_table);
        await table.CreateIfNotExistsAsync();
    }
    public async Task DisposeAsync()
    {
        try
        {
            var client = new TableServiceClient(_connection);
            var table = client.GetTableClient(_table);
            await table.DeleteAsync();
        }
        catch (System.Exception)
        {
        }
    }
    [Fact]
    public async Task GivenEventStream_WhenStoringEvents_ShouldGetThemBack()
    {
        var streamId = "fe8430bf-00a4-42b7-b077-87d8fff4ba68";
        var streamType = "OrderBooking";

        var started = new BookingStarted("", new PurchaseOrder(""))
        {
            EventId = "89795ced-ea64-46c2-879e-10d285a09429",
            SourceId = streamId,
            Version = 1,
        };
        var confirmed = new SalesOrderConfirmed("", new PurchaseOrder(""))
        {
            EventId = "9a5937c2-5e14-461f-b452-fa504f300d15", // unique
            SourceId = streamId,
            Version = 2,
            TargetBranchParentId = started.EventId
        };

        SourcedEvent[] eventStream = [confirmed, started];
        var eventSource = new AzureTableStorageEventSource(_connection, _table);

        await eventSource.Persist(streamType, streamId, eventStream);
        var history = await eventSource.Load(streamType, streamId, 0);

        Assert.Equal(2, history.Count());
        Assert.Equal(started.EventId, history.First().EventId);
        Assert.Equal(confirmed.EventId, history.Last().EventId);

        Assert.IsType<BookingStarted>(history.First());
        Assert.IsType<SalesOrderConfirmed>(history.Last());
        Environment.SetEnvironmentVariable("sarwan", "developer");
    }
    [Fact]
    public async Task GivenEventStream_WhenStoringEvents_ShouldGetThemBack1()
    {
        var streamId = "fe8430bf-00a4-42b7-b077-87d8fff4ba68";
        var streamType = "OrderBooking";

        var started = new BookingStarted("", new PurchaseOrder(""))
        {
            EventId = "89795ced-ea64-46c2-879e-10d285a09429",
            SourceId = streamId,
            Version = 1,
        };
        var confirmed = new SalesOrderConfirmed("", new PurchaseOrder(""))
        {
            EventId = "9a5937c2-5e14-461f-b452-fa504f300d15", // unique
            SourceId = streamId,
            Version = 2,
            TargetBranchParentId = started.EventId
        };

        SourcedEvent[] eventStream = [confirmed, started];
        var eventSource = new AzureTableStorageEventSource(_connection, _table);

        await eventSource.Persist(streamType, streamId, eventStream);
        var history = await eventSource.Load(streamType, streamId, 0);

        Assert.Equal(2, history.Count());
        Assert.Equal(started.EventId, history.First().EventId);
        Assert.Equal(confirmed.EventId, history.Last().EventId);

        Assert.IsType<BookingStarted>(history.First());
        Assert.IsType<SalesOrderConfirmed>(history.Last());
        Environment.SetEnvironmentVariable("sarwan", "developer");
    }
}