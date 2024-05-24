using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace mahjop.Services
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var fromMail = "mahjopmaharma2001@outlook.com";
            var appPassword = "Mahjop@2001"; // Replace with your generated App Password

            using (var message = new MailMessage())
            {
                message.From = new MailAddress(fromMail);
                message.Subject = subject;
                message.To.Add(email);
                message.Body = $"<html><body>{htmlMessage}</body></html>";
                message.IsBodyHtml = true;

                using (var smtpClient = new SmtpClient("smtp-mail.outlook.com"))
                {
                    smtpClient.Port = 587; // Use port 587 for TLS
                    smtpClient.Credentials = new NetworkCredential(fromMail, appPassword);
                    smtpClient.EnableSsl = true;

                    try
                    {
                        await smtpClient.SendMailAsync(message);
                    }
                    catch (Exception ex)
                    {
                        // Handle exceptions or log them
                        Console.WriteLine($"Error sending email: {ex.Message}");
                    }
                }
            }
        }


    }
}
