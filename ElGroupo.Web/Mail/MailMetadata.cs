using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Mail
{
    public class MailMetadata
    {
        public List<string> To { get; set; }
        public string From { get; set; }
        public List<string> Cc { get; set; }
        public List<string> Bcc { get; set; }

        public string Subject { get; set; }


    }
}
