using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Records
{
    public class ViewElementModel
    {
        public bool LabelOnSameRow { get; set; }
        public string DisplayName { get; set; }
        public bool PrimaryDisplay { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public long DataTypeId { get; set; }
        public long? LookupTableId { get; set; }
        public ViewDataTypeModel DataType { get; set; }
        public ViewLookupTableModel LookupTable { get; set; }
        public ViewInputTypeModel InputType { get; set; } 


    }
}
