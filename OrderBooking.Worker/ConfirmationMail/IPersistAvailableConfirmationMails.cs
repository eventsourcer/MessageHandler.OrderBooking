public interface IPersistAvailableConfirmationMails 
{
    Task<ConfirmationMail?> GetAvailableConfirmationMail();

    Task MarkAsSent(ConfirmationMail mail);

    Task MarkAsPending(ConfirmationMail mail);
    Task<ConfirmationMail> GetConfirmationMail(string id);

    Task Insert(ConfirmationMail mail);

    Task Update(ConfirmationMail mail);
}