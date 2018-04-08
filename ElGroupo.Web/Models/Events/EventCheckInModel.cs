using ElGroupo.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Events
{
    public class EventCheckInModel
    {
        public string EventName { get; set; }
        public AttendanceVerificationMethods CheckInMethod { get; set; }
        public long EventId { get; set; }
        public double DistanceTolerance { get; set; }
        public double TimeTolerance { get; set; }
        public double EventCoordX { get; set; }
        public double EventCoordY { get; set; }
        public double UserCoordX { get; set; }
        public double UserCoordY { get; set; }
        public string UserPassword { get; set; }
    }

    public class LocationCheckInModel
    {
        public long EventId { get; set; }
        public double CoordinateX { get; set; }
        public double CoordinateY { get; set; }
    }
    public class PasswordCheckInModel
    {
        public long EventId { get; set; }
        public string Password { get; set; }

    }

    public class SetEventInactiveModel
    {
        public long[] Ids { get; set; }
    }
}
