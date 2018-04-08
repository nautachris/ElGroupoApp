using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElGroupo.Domain
{
    public class AccreditationDocument: ClassBase
    {
        public string ContentType { get; set; }
        public byte[] ImageData { get; set; }
        public virtual ICollection<UserActivityDocument> Activities { get; set; }
    }
}
