using System.Net;

namespace Library.Services.Interfaces
{
    public interface IEmailSenderService
    {
        HttpStatusCode SendEmail(string receiverEmail, string bodyText, string subjectText);
        HttpStatusCode ReceiveEmail(string senderName, string bodyText, string subjectText, string name);
    }
}
