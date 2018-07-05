using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Shared
{
    public class IdValueModel:ReturnViewModel
    {
        public long Id { get; set; }
        public string Value { get; set; }
    }
}
