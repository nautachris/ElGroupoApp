using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Records
{
    public class SaveRecordDocumentsModel
    {
        public long ItemId { get; set; }
        public List<RecordDocumentListModel> Documents { get; set; } = new List<RecordDocumentListModel>();
    }
}
