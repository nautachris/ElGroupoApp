using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Records
{
    public class EditElementsModel
    {
        public List<ViewElementModel> Elements { get; set; } = new List<ViewElementModel>();
        public List<ViewLookupTableModel> LookupTables { get; set; } = new List<ViewLookupTableModel>();
        public List<ViewDataTypeModel> DataTypes { get; set; } = new List<ViewDataTypeModel>();
    }
}
