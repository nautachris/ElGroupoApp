using ElGroupo.Web.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Records
{
    public class EditLookupTableModel
    {
        public string TableName { get; set; }
        public string Description { get; set; }
        public long Id { get; set; }
        public List<IdValueModel> Values { get; set; } = new List<IdValueModel>();
    }
}
