using MessageHandler.EventSourcing.Projections;
using NotificationPreferences.Events;

public class NotificationPreferenceProjection :
        IProjection<Notifications, ConfirmationEmailSet>
{
    public void Project(Notifications model, ConfirmationEmailSet evt)
    {
        model.BuyerId = evt.BuyerId;
        model.EmailAddress = evt.EmailAddress;
    }
}