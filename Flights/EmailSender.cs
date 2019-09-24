using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Flights
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration Configuration;

        public EmailSender(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient(Configuration["SMTP_Server"], int.Parse(Configuration["SMTP_Port"]))
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(Configuration["SenderMail"], Configuration["SenderMailPassword"])
            };
            var mailMessage = new MailMessage
            {
                From = new MailAddress(Configuration["SenderMail"])
            };
            mailMessage.To.Add(email);
            mailMessage.Subject = subject;
            mailMessage.Body = htmlMessage;
            mailMessage.IsBodyHtml = true;
            return client.SendMailAsync(mailMessage);
        }
    }
}
