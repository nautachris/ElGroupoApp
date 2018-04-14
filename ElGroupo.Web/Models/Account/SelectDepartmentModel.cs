using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Account
{
    public class SelectDepartmentModel
    {
        public bool IsSelected { get; set; }
        public string Name { get; set; }
        public long Id { get; set; }
        public List<SelectDepartmentUserGroupModel> Groups = new List<SelectDepartmentUserGroupModel>();
    }
}
