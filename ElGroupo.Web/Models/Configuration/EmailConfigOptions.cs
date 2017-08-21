using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace ElGroupo.Web.Models.Configuration
{
    public class EmailConfigOptions
    {
        public string SendGridUser { get; set; }
        public string SendGridKey { get; set; }

        public string MailgunKey { get; set; }
        public string MailgunDomain { get; set; }

        //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

        public static async Task SendTestEmail()
        {
            try
            {
                var apiKey = "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";
                var client = new SendGridClient(apiKey);
                var from = new EmailAddress("test@example.com", "Example User");
                var subject = "Sending with SendGrid is Fun";
                var to = new EmailAddress("nautachris@gmail.com", "Example User");
                var plainTextContent = "and easy to do anywhere, even with C#";
                var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                var response = await client.SendEmailAsync(msg);
            }
            catch(Exception ex)
            {

            }

        }
    }



}
