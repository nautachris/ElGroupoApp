using System;
using System.Collections.Generic;
using System.Text;

namespace ElGroupo.Domain
{
    public class ContactGroup:ClassBase
    {
        public string Name { get; set; }
        public virtual ICollection<ContactGroupUser> Users { get; set; }
    }
}
