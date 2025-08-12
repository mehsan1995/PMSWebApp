using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Common.Helper
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            //using var client = new SmtpClient("smtp.yourhost.com", 587)
            //{
            //    Credentials = new NetworkCredential("your_email", "your_password"),
            //    EnableSsl = true
            //};

            //var mailMessage = new MailMessage("your_email", email, subject, htmlMessage)
            //{
            //    IsBodyHtml = true
            //};

            //await client.SendMailAsync(mailMessage);
        }
    }

}
