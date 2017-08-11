using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ElGroupo.Domain
{
    public class UserContact: ClassBase
    {
        public long ContactTypeId { get; set; }
        public int UserId { get; set; }

        [Required]
        public virtual Lookups.ContactType ContactType { get; set; }

        [Required]
        public virtual User User { get; set; }

        public string Value { get; set; }
    }
}
