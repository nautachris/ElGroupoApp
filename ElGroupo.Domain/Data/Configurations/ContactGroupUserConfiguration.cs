using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace ElGroupo.Domain.Data.Configurations
{
    public class ContactGroupUserConfiguration : EntityTypeConfiguration<ContactGroupUser>
    {
        public ContactGroupUserConfiguration(ModelBuilder modelBuilder) : base(modelBuilder)
        {
            ToTable("ContactGroupUser", "dbo");
            HasKey(x => x.Id);
            Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();

        }
    }
}
