using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Records
{
    public class CreateRecordItemUserDataModel
    {
        public long RecordElementId { get; set; }
        public long ItemUserId { get; set; }
        public object Value { get; set; }
    }
}
