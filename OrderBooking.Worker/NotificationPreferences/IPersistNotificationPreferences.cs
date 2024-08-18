public interface IPersistNotificationPreferences
{
    public Task<Notifications?> GetNotificationPreferences(string id);

    public Task Insert(Notifications preferences);

    public Task Update(Notifications preferences);
}