using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Options;
using ElGroupo.Web.Models.Configuration;
namespace ElGroupo.Web.Services
{
    public class SendGridEmailSender : IEmailSender
    {
        public EmailConfigOptions Options { get; }
        public SendGridEmailSender(IOptions<EmailConfigOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }
        public async Task<EmailResponse> SendEmailAsync(string emailAddress, string subject, string message)
        {
            var res = await Execute(this.Options.SendGridKey, subject, message, emailAddress);
            return new EmailResponse
            {
                Success = res.StatusCode == (System.Net.HttpStatusCode.OK | System.Net.HttpStatusCode.Accepted),
                Message = await res.Body.ReadAsStringAsync()
            };

        }

        public Task<Response> Execute(string apiKey, string subject, string message, string email)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("andy@elgroupo.com", "Andy O"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));
            return client.SendEmailAsync(msg);
        }
    }
}
