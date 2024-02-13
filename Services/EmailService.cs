using contactPro2.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Security;


namespace contactPro2.Services
{
    public class EmailService : IEmailSender
    {

        private readonly EmailSettings _emailSettings;
      

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {

            try
            {
              

                var emailAddress = _emailSettings.EmailAddress ?? Environment.GetEnvironmentVariable("EmailAddress");
                var emailPasswrd = _emailSettings.EmailPassword ?? Environment.GetEnvironmentVariable("EmailPassword");
                var emailHost = _emailSettings.EmailHost ?? Environment.GetEnvironmentVariable("EmailHost");
                var emailPort = _emailSettings.EmailPort != 0 ? _emailSettings.EmailPort : int.Parse(Environment.GetEnvironmentVariable("EmailPort")!);

                MimeMessage newEmail = new MimeMessage();

                newEmail.Sender = MailboxAddress.Parse(emailAddress);

                // set the recipients
                foreach (string address in email.Split(";"))
                {
                    newEmail.To.Add(MailboxAddress.Parse(address));  // add email address to newEmail
                }

                // set the subject
                newEmail.Subject = subject;

                // set the message
                BodyBuilder emailBody = new BodyBuilder();
                emailBody.HtmlBody = htmlMessage;
                newEmail.Body = emailBody.ToMessageBody();

                // send the email
                using SmtpClient smtpClient = new SmtpClient();

                try
                {
                    await smtpClient.ConnectAsync(emailHost, emailPort, MailKit.Security.SecureSocketOptions.StartTls);
                    await smtpClient.AuthenticateAsync(emailAddress, emailPasswrd);
                    await smtpClient.SendAsync(newEmail);

                    await smtpClient.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(".............. ERROR......................");
                    Console.WriteLine($"Failure sending email with google provider. Error: {ex.Message}");
                    Console.WriteLine(".............. ERROR......................");
                }

            }

            catch (Exception)
            {

                throw;

            }
        }
    }
}

