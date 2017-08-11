using System;
using System.Collections.Generic;
using System.Text;
using ElGroupo.Domain.Lookups;
using Microsoft.EntityFrameworkCore;

namespace ElGroupo.Domain.Data.Configurations
{
    public class ContactTypeConfiguration : EntityTypeConfiguration<ContactType>
    {
        public ContactTypeConfiguration(ModelBuilder modelBuilder) : base(modelBuilder)
        {
            ToTable("ContactTypes", "dbo");
            HasKey(x => x.Id);
            Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
        }
    }
}
