using MessageHandler.EventSourcing.Contracts;

public class BookingChangedToPending(string bookingId) : SourcedEvent
{
    public string BookingID => bookingId;
};