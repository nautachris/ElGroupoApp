using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElGroupo.Domain;
namespace ElGroupo.Web.Models.Shared
{
    public class HeaderViewModel
    {
        public bool IsSignedIn { get; set; }
        public User ActiveUser { get; set; }

        public bool IsAdmin { get; set; }
    }
}
