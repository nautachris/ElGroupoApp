using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Records
{
    public class ViewSubCategoryModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public ViewCategoryModel ParentCategory { get; set; }
        public List<ViewItemModel> RecordItems { get; set; }

                public List<ViewElementModel> DefaultElements { get; set; }
        public List<ViewElementModel> AllElements { get; set; }
    }
}
