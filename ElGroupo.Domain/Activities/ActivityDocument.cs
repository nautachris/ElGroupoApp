﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElGroupo.Domain.Activities
{
    public class ActivityDocument:ClassBase
    {
        public string FileName { get; set; }
        public string Description { get; set; }
        public string ContentType { get; set; }
        public byte[] ImageData { get; set; }
        public Activity Activity { get; set; }
        public long ActivityId { get; set; }
    }
}
