using System;
using System.Collections.Generic;
using System.Text;

namespace ElGroupo.Domain
{
    public class UnregisteredUserConnection
        : ClassBase
    {
        public User User { get; set; }
        public string Phone1Type { get; set; }
        public string Phone1Value { get; set; }
        public string Phone2Type { get; set; }
        public string Phone2Value { get; set; }
        public string Name { get; set; }
        //public Guid RegisterToken { get; set; }
        public string Email { get; set; }
    }
}
