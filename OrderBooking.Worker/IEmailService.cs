public interface IEmailService
{
    Task SendAsync(string message);
}