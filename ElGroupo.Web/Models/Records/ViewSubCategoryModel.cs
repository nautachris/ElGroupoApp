using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElGroupo.Web.Models.Shared;
namespace ElGroupo.Web.Models.Records
{
    public class ViewSubCategoryModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long ParentCategoryId { get; set; }
        public ViewCategoryModel ParentCategory { get; set; }
        public List<ViewItemModel> RecordItems { get; set; }

        public List<ViewElementModel> DefaultElements { get; set; }
        public List<ViewElementModel> AllElements { get; set; }
        public string DescriptionColumnHeader { get; set; }
        public string ValueColumnHeader { get; set; }
    }

    public class SelectSubcategoryModel
    {
        public long ParentCategoryId { get; set; }
        public List<IdValueModel> SubCategories { get; set; } = new List<IdValueModel>();
    }
}
