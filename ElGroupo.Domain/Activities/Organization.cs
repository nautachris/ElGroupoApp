using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElGroupo.Domain.Activities
{
    public class Organization:ClassBase
    {
        public Organization()
        {
            this.Departments = new HashSet<Department>();
            this.Accreditations = new HashSet<OrganizationAccreditation>();
        }
            
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

        public ICollection<Department> Departments { get; set; }
        public ICollection<OrganizationAccreditation> Accreditations { get; set; }

    }
}
