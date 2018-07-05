using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace ElGroupo.Domain.Data.Configurations
{
    public class UserConfiguration : EntityTypeConfiguration<User>
    {
        public UserConfiguration(ModelBuilder modelBuilder) : base(modelBuilder)
        {
            ToTable("User", "dbo");
            HasKey(x => x.Id);
            Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
            Property(x => x.FirstName).HasColumnName("FirstName").HasMaxLength(100);
            Property(x => x.LastName).HasColumnName("LastName").HasMaxLength(100);
            Property(x => x.Title).HasColumnName("Title").HasMaxLength(10);
            Property(x => x.Specialty).HasColumnName("Specialty").HasMaxLength(30);
            Property(x => x.Email).HasColumnName("EmailAddress");
            Property(x => x.TimeZoneId).HasColumnName("TimeZoneId");
            HasMany(x => x.ConnectedUsers).WithOne(x => x.User);
            HasMany(x => x.UnregisteredConnections).WithOne(x => x.User);
            HasMany(x => x.Activities).WithOne(x => x.User).HasForeignKey(x=>x.UserId);
            HasMany(x => x.Departments).WithOne(x => x.User).HasForeignKey(x=>x.UserId);
            HasMany(x => x.OrganizedActivities).WithOne(x => x.User).HasForeignKey(x => x.UserId);
            HasMany(x => x.CustomRecordItems).WithOne(x => x.User).HasForeignKey(x => x.UserId);
            HasMany(x => x.RecordItems).WithOne(x => x.User).HasForeignKey(x => x.UserId);
            //for "private" activity groups
            HasMany(x => x.ActivityGroups).WithOne(x => x.User).HasForeignKey(x => x.UserId);
        }
    }
}
