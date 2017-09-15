using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElGroupo.Web.Mail.Models;
using RazorLight;
using ElGroupo.Web.Mail;

namespace ElGroupo.Web.Services
{
    public class MailService
    {
        private readonly IEmailSender emailSender;
        private readonly IRazorLightEngine engine;
        public MailService(IEmailSender sender, IRazorLightEngine eng)
        {
            this.emailSender = sender;
            this.engine = eng;
        }

        public async Task<EmailResponse> SendEmail(MailMetadata metadata, MailModelBase model)
        {
            return await this.emailSender.SendEmailAsync(metadata.To[0], metadata.Subject, model.RenderEmail(this.engine));
        }

    }
}
