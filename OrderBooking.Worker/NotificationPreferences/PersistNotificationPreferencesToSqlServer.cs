using System.Data;
using Microsoft.Data.SqlClient;

public class PersistNotificationPreferencesToSqlServer : IPersistNotificationPreferences
{
    private string connectionstring;

    public PersistNotificationPreferencesToSqlServer(string connectionstring)
    {
        this.connectionstring = connectionstring;
    }

    private readonly string selectSqlCommand = @"SELECT * FROM [dbo].[NotificationPreferences] WHERE [BuyerId] = @buyerId;";
    private readonly string insertSqlCommand = @"INSERT INTO [dbo].[NotificationPreferences] ([BuyerId], [EmailAddress]) VALUES (@buyerId, @emailAddress);";
    private readonly string updateSqlCommand = @"UPDATE [dbo].[NotificationPreferences] SET [EmailAddress] = @emailAddress Where [BuyerId] = @buyerId;";

    public async Task<Notifications?> GetNotificationPreferences(string id)
    {
        var connection = new SqlConnection(connectionstring);
        connection.Open();

        using var command = new SqlCommand(selectSqlCommand, connection);
        command.Parameters.AddWithValue("@buyerId", id);

        using var dataReader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow);

        if (!await dataReader.ReadAsync())
        {
            return null;
        }

        return await ToNotificationPreferences(dataReader);
    }

    private async Task<Notifications> ToNotificationPreferences(SqlDataReader dataReader)
    {
        return new Notifications
        {
            BuyerId = await dataReader.GetFieldValueAsync<string>(0),
            EmailAddress = await dataReader.GetFieldValueAsync<string>(1)
        };
    }

    public async Task Insert(Notifications preferences)
    {
        var connection = new SqlConnection(connectionstring);
        connection.Open();

        using var command = new SqlCommand(insertSqlCommand, connection);
        command.Parameters.AddWithValue("@buyerId", preferences.BuyerId);
        command.Parameters.AddWithValue("@emailAddress", preferences.EmailAddress);
        await command.ExecuteNonQueryAsync();
    }

    public async Task Update(Notifications preferences)
    {
        var connection = new SqlConnection(connectionstring);
        connection.Open();

        using var command = new SqlCommand(updateSqlCommand, connection);
        command.Parameters.AddWithValue("@buyerId", preferences.BuyerId);
        command.Parameters.AddWithValue("@emailAddress", preferences.EmailAddress);
        await command.ExecuteNonQueryAsync();
    }
}