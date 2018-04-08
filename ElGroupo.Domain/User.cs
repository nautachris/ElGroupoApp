using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using ElGroupo.Domain.Lookups;
namespace ElGroupo.Domain
{
    public class User : IdentityUser<long>
    {
        public virtual ICollection<Activity> Activities { get; set; }
        public User()
        {
            this.ContactMethods = new HashSet<UserContactMethod>();
            this.ConnectedUsers = new HashSet<UserConnection>();
            this.UnregisteredConnections = new HashSet<UnregisteredUserConnection>();
        }

        public void AddContact(Lookups.ContactMethod type, string value)
        {
            var uc = new UserContactMethod();
            uc.User = this;
            uc.ContactMethod = type;
            uc.Value = value;
            this.ContactMethods.Add(uc);
        }


        public ICollection<UserActivity> CMEActivities { get; set; }
        public ICollection<UserActivity> NonCMEActivities { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual ICollection<UnregisteredUserConnection> UnregisteredConnections { get; set; }
        public virtual ICollection<UserContactMethod> ContactMethods { get; set; }

        public virtual ICollection<UserConnection> ConnectedUsers { get; set; }

        //public virtual ICollection<EventOrganizer> OrganizedEvents { get; set; }
        public virtual ICollection<EventAttendee> AttendedEvents { get; set; }

        public virtual ICollection<AttendeeGroup> AttendeeGroups { get; set; }


        public virtual UserPhoto Photo { get; set; }

        public long? PhotoId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }
        public string ZipCode { get; set; }
        
        [MaxLength(100)]
        public string TimeZoneId { get; set; }
    }
}
