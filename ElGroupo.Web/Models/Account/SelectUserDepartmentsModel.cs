using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Account
{
    public class SelectUserDepartmentsModel
    {
        public List<OrganizationListModel> Organizations { get; set; } = new List<OrganizationListModel>();

    }
}
