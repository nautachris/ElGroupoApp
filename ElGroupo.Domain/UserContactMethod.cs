using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ElGroupo.Domain
{
    public class UserContactMethod: ClassBase
    {
        public long ContactMethodId { get; set; }
        public long UserId { get; set; }

        [Required]
        public virtual Lookups.ContactMethod ContactMethod { get; set; }

        [Required]
        public virtual User User { get; set; }

        public string Value { get; set; }
    }
}
