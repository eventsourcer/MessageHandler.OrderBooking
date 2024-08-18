public class SendAvailableConfirmationEmails
{
    private readonly ILogger<SendAvailableConfirmationEmails> logger;
    private readonly IEmailService emailSender;
    private readonly IPersistAvailableConfirmationMails storage;

    public SendAvailableConfirmationEmails(IPersistAvailableConfirmationMails storage, IEmailService emailSender, ILogger<SendAvailableConfirmationEmails> logger = null!)
    {
        this.logger = logger;
        this.emailSender = emailSender;
        this.storage = storage;
    }

    public async Task ProcessAsync(CancellationToken stoppingToken)
    {
        var email = await storage.GetAvailableConfirmationMail();

        if (email != null)
        {
            try
            {
                logger?.LogInformation("Confirmation mail available, sending it...");

                await emailSender.SendAsync(email.SenderEmailAddress,
                                                        email.BuyerEmailAddress,
                                                        email.EmailSubject,
                                                        email.EmailBody);

                await storage.MarkAsSent(email);

                logger?.LogInformation("Confirmation mail marked as sent...");
            }
            catch (Exception e)
            {
                await storage.MarkAsPending(email);

                if(logger.IsEnabled(LogLevel.Error))
                    logger.LogError($"Sending confirmation mail failed, marked as pending...Error: {e.Message}");
            }
        }
        else
        {
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}