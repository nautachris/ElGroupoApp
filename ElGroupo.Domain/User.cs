using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ElGroupo.Domain.Lookups;
namespace ElGroupo.Domain
{
    public class User : IdentityUser<int>
    {

        public User()
        {
            this.Contacts = new HashSet<UserContact>();

        }

        public void AddContact(Lookups.ContactType type, string value)
        {
            var uc = new UserContact();
            uc.User = this;
            uc.ContactType = type;
            uc.Value = value;
            this.Contacts.Add(uc);
        }


        public virtual ICollection<UserContact> Contacts { get; set; }
        public virtual ICollection<ContactGroup> ContactGroups { get; set; }
        public virtual ICollection<EventOrganizer> OrganizedEvents { get; set; }
        public virtual ICollection<EventAttendee> AttendedEvents { get; set; }
        public virtual ICollection<EventGroup> EventGroups { get; set; }

        public virtual UserPhoto Photo { get; set; }

        public long? PhotoId { get; set; }
        public string Name { get; set; }
        public string ZipCode { get; set; }
    }
}
