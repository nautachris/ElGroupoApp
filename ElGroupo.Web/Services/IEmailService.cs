using ElGroupo.Web.Mail;
using ElGroupo.Web.Mail.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Services
{
    public interface IEmailService
    {
        Task<EmailResponse> SendEmail(MailMetadata metadata, MailModelBase model);
    }
}
