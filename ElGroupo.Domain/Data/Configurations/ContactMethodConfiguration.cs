using System;
using System.Collections.Generic;
using System.Text;
using ElGroupo.Domain.Lookups;
using Microsoft.EntityFrameworkCore;

namespace ElGroupo.Domain.Data.Configurations
{
    public class ContactMethodConfiguration : EntityTypeConfiguration<ContactMethod>
    {
        public ContactMethodConfiguration(ModelBuilder modelBuilder) : base(modelBuilder)
        {
            ToTable("ContactMethods", "dbo");
            HasKey(x => x.Id);
            Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();

        }
    }
}
