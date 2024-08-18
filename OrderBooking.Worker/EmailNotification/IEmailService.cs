public interface IEmailService
{
    Task SendAsync(string message);
    Task SendAsync(string sellerEmail, string buyeremail, string subject, string body);
}