using ElGroupo.Web.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Records
{
    public class NewUserRecordModel : ReturnViewModel
    {
        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public long? CategoryId { get; set; }
        public long? SubCategoryId { get; set; }

        public List<CreateItemUserDataModel> UserData { get; set; } = new List<CreateItemUserDataModel>();
        public List<IdValueModel> ItemTypes { get; set; } = new List<IdValueModel>();
        public string Name { get; set; }
        public long? RecordItemId { get; set; }

        //public List<RecordDocumentListModel> Documents { get; set; } = new List<RecordDocumentListModel>();
    }
}
