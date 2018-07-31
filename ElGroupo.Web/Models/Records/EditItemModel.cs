using ElGroupo.Domain.Lookups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Records
{
    public class EditItemModel
    {
        public List<IdValueModel> ItemTypes { get; set; } = new List<IdValueModel>();
        public long? ItemTypeId { get; set; }
        public string Name { get; set; }
        public long? CategoryId { get; set; }
        public long? SubCategoryId { get; set; }
        public List<EditItemUserDataModel> UserData { get; set; } = new List<EditItemUserDataModel>();
        public long ItemId { get; set; }
        public List<RecordDocumentListModel> Documents { get; set; } = new List<RecordDocumentListModel>();
    }
}
