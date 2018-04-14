using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElGroupo.Domain.Activities
{
    public class CreditType: ClassBase
    {
        public string Description { get; set; }
        public virtual ICollection<CreditTypeCategory> Categories { get; set; }
    }
}
