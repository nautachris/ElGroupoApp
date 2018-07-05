using ElGroupo.Web.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Records
{
    public class CreateRecordItemElementModel:ReturnViewModel
    {
        public long RecordItemId { get; set; }
        public long ElementId { get; set; }
        public bool PrimaryDisplay { get; set; }
    }
}
