using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Users
{
    public class ConnectionAutoCompleteModel: AutoCompleteModel
    {
        public bool Registered { get; set; }
        public bool Group { get; set; }
        public int GroupUserCount { get; set; }
    }
}
