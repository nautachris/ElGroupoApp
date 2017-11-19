using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElGroupo.Web.Mail.Models;
using RazorLight;
using ElGroupo.Web.Mail;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using ElGroupo.Web.Models.Configuration;
using Microsoft.Extensions.Options;

namespace ElGroupo.Web.Services
{
    public class MailService : IEmailService
    {

        public MailService() { }



        private readonly IAmazonSimpleEmailService emailSender;
        //private readonly IEmailSender emailSender;
        private readonly IRazorLightEngine engine;
        private readonly AmazonSESConfig mailConfig;
        public MailService(IAmazonSimpleEmailService sender, IRazorLightEngine eng, IOptions<AmazonSESConfig> opts)
        {
            this.emailSender = sender;
            this.engine = eng;
            this.mailConfig = opts.Value;
        }

        public async Task<EmailResponse> SendEmail(MailMetadata metadata, MailModelBase model)
        {
            //this.emailSender.se

            var req = new SendEmailRequest
            {
                Destination = new Destination(metadata.To),
                Source = this.mailConfig.Sender,
                Message = new Message(new Content(metadata.Subject), new Body(new Content(model.RenderEmail(this.engine))))
            };

            var resp = await this.emailSender.SendEmailAsync(req);
            if (resp.HttpStatusCode == System.Net.HttpStatusCode.Accepted || resp.HttpStatusCode == System.Net.HttpStatusCode.OK) return new EmailResponse { Success = true };
            return new EmailResponse { Success = false, Message = string.Join(", ", resp.ResponseMetadata.Metadata.Select(x => x.Key + " - " + x.Value)) };
        }

    }
}
