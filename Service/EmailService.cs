using MailKit.Net.Smtp;
using MimeKit;
using System;

namespace ecommerce_final
{
    public class EmailService
    {
        private const string SmtpServer = "smtp.gmail.com";
        private const int SmtpPort = 587; // Sử dụng TLS
        private const string EmailAddress = "lobbystu.education@gmail.com";
        private const string EmailPassword = "xhfq amwr dyrt ixwh"; // Hoặc mật khẩu Gmail

        public void SendEmail(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("ECS", EmailAddress));
            email.To.Add(new MailboxAddress("", toEmail));
            email.Subject = subject;

            // Nội dung email
            email.Body = new TextPart("plain")
            {
                Text = body
            };

            using (var smtp = new SmtpClient())
            {
                try
                {
                    smtp.Connect(SmtpServer, SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                    smtp.Authenticate(EmailAddress, EmailPassword);
                    smtp.Send(email);
                    Console.WriteLine("Email sent successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending email: {ex.Message}");
                }
                finally
                {
                    smtp.Disconnect(true);
                }
            }
        }
    }
}
