using Library.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Net;

namespace Library.Services.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        public IConfiguration _configuration { get; }
        public EmailSenderService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public HttpStatusCode SendEmail(string receiverEmail, string bodyText, string subjectText)
        {
            var email = new MimeMessage();

            var usernameSecret = _configuration.GetSection("UserEmailSecrets:Username").Get<string>() ?? "";
            var passwordSecret = _configuration.GetSection("UserEmailSecrets:Password").Get<string>() ?? "";

            email.From.Add(new MailboxAddress("Sender Name", usernameSecret));
            email.To.Add(new MailboxAddress("Receiver Name", receiverEmail));

            email.Subject = subjectText;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = bodyText
            };

            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                bool emailSentSuccessfully = false;

                smtp.MessageSent += (sender, args) =>
                {
                    if (args.Response != null && args.Response.Contains("250 2.0.0 OK"))
                    {
                        emailSentSuccessfully = true;
                    }
                    else
                    {
                        emailSentSuccessfully = false;
                    }
                };

                try
                {
                    smtp.Connect("smtp.gmail.com", 587, false);
                    smtp.Authenticate(usernameSecret, passwordSecret);
                    smtp.Send(email);
                    emailSentSuccessfully = true;
                }
                catch (Exception ex)
                {
                    emailSentSuccessfully = false;
                }
                finally
                {
                    smtp.Disconnect(true);
                }

                return emailSentSuccessfully ? HttpStatusCode.OK : HttpStatusCode.BadRequest;
            }
        }

        public HttpStatusCode ReceiveEmail(string senderName, string bodyText, string subjectText, string name)
        {
            var email = new MimeMessage();

            var usernameSecret = _configuration.GetSection("UserEmailSecrets:Username").Get<string>() ?? "";
            var passwordSecret = _configuration.GetSection("UserEmailSecrets:Password").Get<string>() ?? "";

            email.To.Add(new MailboxAddress("Receiver Name", usernameSecret));
            email.From.Add(new MailboxAddress("Sender Name", senderName));

            email.Subject = subjectText;
            bodyText = $"{name}\r\n{bodyText}";
            email.Body = new TextPart(MimeKit.Text.TextFormat.Plain)
            {
                Text = bodyText
            };

            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                bool emailSentSuccessfully = false;

                smtp.MessageSent += (sender, args) =>
                {
                    if (args.Response != null && args.Response.Contains("250 2.0.0 OK"))
                    {
                        emailSentSuccessfully = true;
                    }
                    else
                    {
                        emailSentSuccessfully = false;
                    }
                };

                try
                {
                    smtp.Connect("smtp.gmail.com", 587, false);
                    smtp.Authenticate(usernameSecret, passwordSecret);
                    smtp.Send(email);
                    emailSentSuccessfully = true;
                }
                catch (Exception ex)
                {
                    emailSentSuccessfully = false;
                }
                finally
                {
                    smtp.Disconnect(true);
                }

                return emailSentSuccessfully ? HttpStatusCode.OK : HttpStatusCode.BadRequest;
            }
        }
    }
}
