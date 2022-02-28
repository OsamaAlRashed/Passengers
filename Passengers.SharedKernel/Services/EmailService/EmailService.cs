using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.SharedKernel.Services.EmailService
{
    public class EmailService
    {
        private readonly MailOptions options;

        public EmailService(IOptions<MailOptions> options)
        {
            this.options = options?.Value;
        }

        public async Task<bool> SendEmail(Message m) => await SendEmail(m ,options);

        public static async Task<bool> SendEmail(Message m, MailOptions options)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Passengers App", options.Mail));
                message.To.Add(new MailboxAddress("" , m.To));
                message.Subject = m.Subject;
                message.Body = message.Body = new TextPart("plain")
                {
                    Text = m.Body
                }; 
                using (var emailClient = new SmtpClient())
                {
                    emailClient.Connect("smtp.gmail.com", 465, MailKit.Security.SecureSocketOptions.SslOnConnect);
                    await emailClient.AuthenticateAsync(options.Mail, options.Password);
                    emailClient.Send(message);
                    emailClient.Disconnect(true);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
