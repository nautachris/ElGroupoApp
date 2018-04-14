using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Account
{
    public class EditUserDepartmentsModel
    {
        public long DepartmentId { get; set; }
        public long[] GroupIds { get; set; }
    }
}
