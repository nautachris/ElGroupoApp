using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Mail.Models
{
    public class ForgotPasswordMailModel : MailModelBase
    {
        public override string TemplateName => "ForgotPassword";
    }
}
