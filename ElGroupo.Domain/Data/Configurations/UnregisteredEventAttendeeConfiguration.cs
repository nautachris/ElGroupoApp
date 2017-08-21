using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
namespace ElGroupo.Domain.Data.Configurations
{
    public class UnregisteredEventAttendeeConfiguration: EntityTypeConfiguration<UnregisteredEventAttendee>
    {
        public UnregisteredEventAttendeeConfiguration(ModelBuilder builder) : base(builder)
        {
            ToTable("UnregisteredEventAttendees", "dbo");
            HasKey(x => x.Id);
            Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
            HasOne(x => x.Event).WithMany(y => y.UnregisteredAttendees).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);
        }
    }
}
