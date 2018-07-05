using ElGroupo.Web.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Records
{
    public class CreateRecordItemModel:ReturnViewModel
    {
        public string Name { get; set; }
        public long? CategoryId { get; set; }
        public long? SubCategoryId { get; set; }
    }

    public class EditRecordItemModel : CreateRecordItemModel
    {
        public long Id { get; set; }
    }
}
