using System;
using System.Collections.Generic;
using System.Text;

namespace ElGroupo.Domain
{
    public class ContactGroupUser:ClassBase
    {
        public long GroupId { get; set; }
        public int UserId { get; set; }
        public virtual ContactGroup Group { get; set; }

        public virtual User User { get; set; }
    }
}
