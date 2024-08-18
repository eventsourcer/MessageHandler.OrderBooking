using System.Data;
using Microsoft.Data.SqlClient;

public class PersistAvailableConfirmationMails : IPersistAvailableConfirmationMails
{
    private string connectionstring;
    private readonly string updateAvailableConfirmationMailSqlCommand = @"UPDATE [dbo].[SalesOrderConfirmations] SET[Status] = @status Where [OrderId] = @orderId;"; 


    public PersistAvailableConfirmationMails(string connectionstring)
    {
        this.connectionstring = connectionstring;
    }

    private readonly string getAvailableConfirmationMailSqlCommand = 
@"WITH task AS (
SELECT TOP(1) [dbo].[SalesOrderConfirmations].*, [dbo].[NotificationPreferences].EmailAddress as BuyerEmailAddress
FROM[dbo].[SalesOrderConfirmations]
        INNER JOIN[dbo].[NotificationPreferences] on[dbo].[SalesOrderConfirmations].[BuyerId] = [dbo].[NotificationPreferences].[BuyerId]
        WHERE[dbo].[SalesOrderConfirmations].[Status] = 'Pending')
UPDATE task
SET [Status] = 'Processing'
OUTPUT
    deleted.OrderId,
    deleted.BuyerId,
    deleted.SenderEmailAddress,
    deleted.BuyerEmailAddress,
    deleted.EmailSubject,
    deleted.EmailBody,
    inserted.Status;";

    public async Task<ConfirmationMail?> GetAvailableConfirmationMail()
    {
        var connection = new SqlConnection(connectionstring);
        connection.Open();

        using var command = new SqlCommand(getAvailableConfirmationMailSqlCommand, connection);
        using var dataReader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow);

        if (!await dataReader.ReadAsync())
        {
            return null;
        }

        return await ToConfirmationMail1(dataReader);
    }

    private async Task<ConfirmationMail> ToConfirmationMail1(SqlDataReader dataReader)
    {
        return new ConfirmationMail
        {
            OrderId = await dataReader.GetFieldValueAsync<string>(0),
            BuyerId = await dataReader.GetFieldValueAsync<string>(1),
            SenderEmailAddress = await dataReader.GetFieldValueAsync<string>(2),
            BuyerEmailAddress = await dataReader.GetFieldValueAsync<string>(3),
            EmailSubject = await dataReader.GetFieldValueAsync<string>(4),
            EmailBody = await dataReader.GetFieldValueAsync<string>(5),
            Status = await dataReader.GetFieldValueAsync<string>(6)
        };
    }
    private async Task<ConfirmationMail> ToConfirmationMail2(SqlDataReader dataReader)
    {
        return new ConfirmationMail
        {
            OrderId = await dataReader.GetFieldValueAsync<string>(0),
            BuyerId = await dataReader.GetFieldValueAsync<string>(1),
            SenderEmailAddress = await dataReader.GetFieldValueAsync<string>(2),
            // BuyerEmailAddress = await dataReader.GetFieldValueAsync<string>(3),
            EmailSubject = await dataReader.GetFieldValueAsync<string>(3),
            EmailBody = await dataReader.GetFieldValueAsync<string>(4),
            Status = await dataReader.GetFieldValueAsync<string>(5)
        };
    }
    public async Task MarkAsSent(ConfirmationMail mail)
    {
        var connection = new SqlConnection(connectionstring);
        connection.Open();

        using var command = new SqlCommand(updateAvailableConfirmationMailSqlCommand, connection);
        command.Parameters.AddWithValue("@status", "Sent");
        command.Parameters.AddWithValue("@orderId", mail.OrderId);
        await command.ExecuteNonQueryAsync();
    }
    public async Task MarkAsPending(ConfirmationMail mail)
    {
        var connection = new SqlConnection(connectionstring);
        connection.Open();

        using var command = new SqlCommand(updateAvailableConfirmationMailSqlCommand, connection);
        command.Parameters.AddWithValue("@status", "Pending");
        command.Parameters.AddWithValue("@orderId", mail.OrderId);
        await command.ExecuteNonQueryAsync();
    }
    private readonly string selectSqlCommand = @"SELECT * FROM [dbo].[SalesOrderConfirmations] WHERE [OrderId] = @orderId;";
    private readonly string insertSqlCommand = @"INSERT INTO [dbo].[SalesOrderConfirmations] ([OrderId], [BuyerId], [SenderEmailAddress], [EmailSubject], [EmailBody], [Status]) VALUES (@orderId, @buyerId, @senderEmailAddress, @emailSubject, @emailBody, @status);";
    private readonly string updateSqlCommand = @"UPDATE [dbo].[SalesOrderConfirmations] SET [Status] = @status Where [OrderId] = @orderId;";
    public async Task<ConfirmationMail> GetConfirmationMail(string id)
    {
        var connection = new SqlConnection(connectionstring);
        connection.Open();

        using var command = new SqlCommand(selectSqlCommand, connection);
        command.Parameters.AddWithValue("@orderId", id);

        using var dataReader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow);

        if (!await dataReader.ReadAsync())
        {
            return new ConfirmationMail();
        }

        return await ToConfirmationMail1(dataReader);
    }

    public async Task Insert(ConfirmationMail mail)
    {
        var connection = new SqlConnection(connectionstring);
        connection.Open();

        using var command = new SqlCommand(insertSqlCommand, connection);
        command.Parameters.AddWithValue("@orderId", mail.OrderId);
        command.Parameters.AddWithValue("@buyerId", mail.BuyerId);
        command.Parameters.AddWithValue("@senderEmailAddress", mail.SenderEmailAddress);
        command.Parameters.AddWithValue("@emailSubject", mail.EmailSubject);
        command.Parameters.AddWithValue("@emailBody", mail.EmailBody);
        command.Parameters.AddWithValue("@status", "Draft");
        await command.ExecuteNonQueryAsync();
    }

    public async Task Update(ConfirmationMail mail)
    {
        var connection = new SqlConnection(connectionstring);
        connection.Open();

        using var command = new SqlCommand(updateSqlCommand, connection);
        command.Parameters.AddWithValue("@status", mail.Status);
        command.Parameters.AddWithValue("@orderId", mail.OrderId);
        await command.ExecuteNonQueryAsync();
    }
}

public class ConfirmationMail
{
    public string OrderId { get; set; } = string.Empty;
    public string BuyerId { get; set; } = string.Empty;
    public string SenderEmailAddress { get; set; } = string.Empty;
    public string BuyerEmailAddress { get; set; } = string.Empty;
    public string EmailSubject { get; set; } = string.Empty;
    public string EmailBody { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}