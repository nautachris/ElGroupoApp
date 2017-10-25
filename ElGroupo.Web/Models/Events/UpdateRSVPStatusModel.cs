using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElGroupo.Domain.Enums;
namespace ElGroupo.Web.Models.Events
{
    public class UpdateRSVPStatusModel
    {
        public RSVPTypes Status { get; set; }
        public long EventId { get; set; }
    }
}
