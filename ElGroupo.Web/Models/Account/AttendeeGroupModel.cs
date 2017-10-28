using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Account
{
    public class AttendeeGroupModel
    {
        public long UserId { get; set; }
        public string Name { get; set; }
        public long Id { get; set; }
        public List<AttendeeGroupUserModel> Users { get; set; } = new List<AttendeeGroupUserModel>();
    }
}
