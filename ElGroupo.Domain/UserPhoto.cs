using System;
using System.Collections.Generic;
using System.Text;

namespace ElGroupo.Domain
{
    public class UserPhoto:ClassBase
    {
        public string ContentType { get; set; }
        public byte[] ImageData { get; set; }
    }
}
