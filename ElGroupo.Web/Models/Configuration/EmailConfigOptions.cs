using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace ElGroupo.Web.Models.Configuration
{
    public class AmazonSESConfig
    {
        public string AmazonEmailKey { get; set; }
        public string AmazonEmailSecret { get; set; }

        public string Sender { get; set; }


        //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


    }



}
