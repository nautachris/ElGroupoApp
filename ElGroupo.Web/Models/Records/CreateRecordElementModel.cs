using ElGroupo.Web.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Records
{
    public class CreateRecordElementModel:IdModel
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public long DataTypeId { get; set; }
        public long? LookupTableId { get; set; }
        public long? LookupTableFieldTypeId { get; set; }
        public long InputTypeId { get; set; }
        public bool SameRow { get; set; }
    }
}
