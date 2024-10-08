namespace OrderBooking.Worker;

public class ConfirmationMailWorker(
    ILogger<ConfirmationMailWorker> logger,
    SendAvailableConfirmationEmails processor) : BackgroundService
{
    private readonly ILogger<ConfirmationMailWorker> _logger = logger;
    private readonly SendAvailableConfirmationEmails _processor = processor;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _processor.ProcessAsync(stoppingToken);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "{Message}", ex.Message);
        }
    }
}
