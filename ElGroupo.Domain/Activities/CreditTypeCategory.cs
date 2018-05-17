using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElGroupo.Domain.Activities
{
    public class CreditTypeCategory:ClassBase
    {
        public string Description { get; set; }
        public long CreditTypeId { get; set; }
        public CreditType CreditType { get; set; }
        public bool Active { get; set; }
    }
}
