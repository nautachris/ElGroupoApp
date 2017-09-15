using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RazorLight;
namespace ElGroupo.Web.Mail.Models
{
    public abstract class MailModelBase : IMailModel
    {
        public abstract string TemplateName { get; }
        public string Recipient { get; set; }
        public string CallbackUrl { get; set; }
        public string RenderEmail(IRazorLightEngine engine)
        {
            try
            {
                return engine.Parse(TemplateName, this);
            }
            catch (Exception ex)
            {
                return null;
            }

        }
    }
}
