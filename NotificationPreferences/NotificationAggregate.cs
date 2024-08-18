using MessageHandler.EventSourcing.DomainModel;
using NotificationPreferences.Events;

namespace NotificationPreferences;

public class NotificationAggregate : EventSourced, IApply<ConfirmationEmailSet>
{
    private string _email = string.Empty;
    public NotificationAggregate() : base(Guid.NewGuid().ToString())
    {
        
    }
    public NotificationAggregate(string id) : base(id)
    {
        
    }
    public void Apply(ConfirmationEmailSet confirmationEmailSet)
    {
        _email = confirmationEmailSet.EmailAddress;
    }
    public void SetConfirmationEmail(string email)
    {
        if (_email == email) return;

        Emit(new ConfirmationEmailSet(Id, email));
    }
}
