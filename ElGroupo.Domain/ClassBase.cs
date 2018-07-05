using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ElGroupo.Domain
{
    public abstract class ClassBase
    {
        [Key]
        public long Id { get; set; }
        //public DateTime DateCreated { get; set; }
        //public string UserCreated { get; set; }
        //public DateTime DateUpdated { get; set; }
        //public string UserUpdated { get; set; }
    }
}
