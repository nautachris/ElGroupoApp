using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Account
{
    public class AddDepartmentModel
    {
        public long OrganizationId { get; set; }
        public string DepartmentName { get; set; }
    }
    public class AddDepartmentGroupModel
    {
        public long DepartmentId { get; set; }
        public string GroupName { get; set; }
    }

    public class DeleteDepartmentModel
    {
        public long DepartmentId { get; set; }
    }

        public class DeleteDepartmentGroupModel
    {
        public long GroupId { get; set; }
    }
}
