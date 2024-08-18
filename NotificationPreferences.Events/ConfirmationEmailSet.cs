using MessageHandler.EventSourcing.Contracts;

namespace NotificationPreferences.Events;

public class ConfirmationEmailSet(string buyerId, string emailAddress) : SourcedEvent
{
    public string BuyerId => buyerId;
    public string EmailAddress => emailAddress;
}
