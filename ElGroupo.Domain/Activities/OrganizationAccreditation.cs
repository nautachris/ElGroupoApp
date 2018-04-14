using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElGroupo.Domain.Activities
{
    public class OrganizationAccreditation:ClassBase
    {
        public long OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public long CreditTypeId { get; set; }
        public CreditType CreditType { get; set; }
        public string AuthorityId { get; set; }
    }
}
