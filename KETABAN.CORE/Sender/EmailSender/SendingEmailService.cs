using MailKit.Net.Smtp;
using MimeKit;
using System;

namespace KETABAN.CORE.Sender.EmailSender
{
    public class SendingEmailService
    {
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Ketaban", "hasanlou969@gmail.com"));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = body
            };
            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                
                
                   await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);

                    // Note: only needed if the SMTP server requires authentication
                   await client.AuthenticateAsync("Emailaddress@gmail.com", "password");

                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                
            }
        }
    }
}
