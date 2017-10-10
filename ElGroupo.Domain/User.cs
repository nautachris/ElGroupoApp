﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ElGroupo.Domain.Lookups;
namespace ElGroupo.Domain
{
    public class User : IdentityUser<long>
    {

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

        public virtual ICollection<UnregisteredUserConnection> UnregisteredConnections { get; set; }
        public virtual ICollection<UserContactMethod> ContactMethods { get; set; }

        public virtual ICollection<UserConnection> ConnectedUsers { get; set; }

        //public virtual ICollection<EventOrganizer> OrganizedEvents { get; set; }
        public virtual ICollection<EventAttendee> AttendedEvents { get; set; }
        public virtual ICollection<EventGroup> EventGroups { get; set; }

        public virtual UserPhoto Photo { get; set; }

        public long? PhotoId { get; set; }
        public string Name { get; set; }
        public string ZipCode { get; set; }
    }
}
