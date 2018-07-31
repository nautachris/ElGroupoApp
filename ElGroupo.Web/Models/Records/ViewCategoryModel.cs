using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Records
{
    public class ViewCategoryModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<ViewItemModel> RecordItems { get; set; }
        public List<ViewSubCategoryModel> SubCategories { get; set; }
        public List<ViewElementModel> DefaultElements { get; set; }
        public List<ViewElementModel> AllElements { get; set; }

        public string DescriptionColumnHeader { get; set; }
        public string ValueColumnHeader { get; set; }
    }
}
