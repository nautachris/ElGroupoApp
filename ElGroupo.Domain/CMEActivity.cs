using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElGroupo.Domain
{
    public class CMEActivity:Activity
    {
        public virtual CMEActivityType CMEType { get; set; }
        public virtual Conference Conference { get; set; }


        public double CMEHours { get; set; } = 0;

        //ie MRI, Ultrasound (see 23rd annual radiology in the desert cerficiate for allyson richards)
        public string Topic { get; set; }

        public string Instructor { get; set; }
      
        public string CMEActivityCode { get; set; }


    }
}
