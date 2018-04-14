using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Account
{
    public class OrganizationListModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
        public List<SelectDepartmentModel> Departments { get; set; } = new List<SelectDepartmentModel>();
        

    }
}
