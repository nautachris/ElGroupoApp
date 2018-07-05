using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Records
{
    public class ViewItemModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long? CategoryId { get; set; }
        public long? SubCategoryId { get; set; }

        public ViewCategoryModel Category { get; set; }
        public ViewSubCategoryModel SubCategory { get; set; }
        public List<ViewElementModel> Elements { get; set; } = new List<ViewElementModel>();

        public List<ViewElementModel> ElementList { get; set; } = new List<ViewElementModel>();
    }
}
