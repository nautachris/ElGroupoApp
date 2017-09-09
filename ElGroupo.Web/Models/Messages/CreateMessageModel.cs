using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Messages
{
    public class CreateMessageModel
    {
        public string Subject { get; set; }
        public string Text { get; set; }
        public long EventId { get; set; }
    }
}
