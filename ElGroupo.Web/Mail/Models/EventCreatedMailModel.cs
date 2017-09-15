using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Mail.Models
{
    public class EventCreatedMailModel:EventMailModel
    {




        public override string TemplateName
        {
            get
            {
                return "EventCreated";
            }
        }
    }
}
